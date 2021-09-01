/************************************************************************************
 ¡¾PXR SDK¡¿
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class PXR_DirectorySyncer
{
	public delegate void SyncResultDelegate(SyncResult syncResult);

	public readonly string source;
	public readonly string target;
	public SyncResultDelegate willPerformOperations;
	private readonly Regex ignoreExpression;

	// helper classes to simplify transition beyond .NET runtime 3.5
	public abstract class CancellationToken
	{
		protected abstract bool CancellationRequestedState();

		public virtual bool IsCancellationRequested
		{
			get { return CancellationRequestedState(); }
		}

		public void ThrowIfCancellationRequested()
		{
			if (IsCancellationRequested)
			{
				throw new Exception("Operation Cancelled");
			}
		}

		public static readonly CancellationToken None = new CancellationTokenNone();

		private class CancellationTokenNone : CancellationToken
		{
			protected override bool CancellationRequestedState()
			{
				return false;
			}
		}
	}

	public class CancellationTokenSource : CancellationToken
	{
		private bool isCancelled;

		protected override bool CancellationRequestedState()
		{
			return isCancelled;
		}

		public void Cancel()
		{
			isCancelled = true;
		}

		public CancellationToken Token
		{
			get { return this; }
		}
	}

	private static string EnsureTrailingDirectorySeparator(string path)
	{
		return path.EndsWith("" + Path.DirectorySeparatorChar)
			? path
			: path + Path.DirectorySeparatorChar;
	}

	public static string CreateDirectory(string path1, string path2 = null)
    {
		string path = path1;
        if (path2 != null)
        {
			path = Path.Combine(path1, path2);
        }

		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		return path;
	}

	private static string CheckedDirectory(string nameInExceptionText, string directory)
	{
		directory = Path.GetFullPath(directory);
		if (!Directory.Exists(directory))
		{
			throw new ArgumentException(string.Format("{0} is not a valid directory for argument ${1}", directory,
				nameInExceptionText));
		}

		return EnsureTrailingDirectorySeparator(directory);
	}

	public PXR_DirectorySyncer(string newSource, string newTarget, string ignoreRegExPattern = null)
	{
		source = CheckedDirectory("newSource", newSource);
		target = CheckedDirectory("target", newTarget);
		if (source.StartsWith(target, StringComparison.OrdinalIgnoreCase) ||
			target.StartsWith(source, StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException(string.Format("Paths must not contain each other (newSource: {0}, target: {1}",
				source, target));
		}

		ignoreRegExPattern = ignoreRegExPattern ?? "^$";
		ignoreExpression = new Regex(ignoreRegExPattern, RegexOptions.IgnoreCase);
	}

	public class SyncResult
	{
		public readonly IEnumerable<string> created;
		public readonly IEnumerable<string> updated;
		public readonly IEnumerable<string> deleted;

		public SyncResult(IEnumerable<string> newCreated, IEnumerable<string> newUpdated, IEnumerable<string> newDeleted)
		{
			created = newCreated;
			updated = newUpdated;
			deleted = newDeleted;
		}
	}

	public bool RelativeFilePathIsRelevant(string relativeFilename)
	{
		return !ignoreExpression.IsMatch(relativeFilename);
	}

	public bool RelativeDirectoryPathIsRelevant(string relativeDirName)
	{
		// Since our ignore patterns look at file names, they may contain trailing path separators
		// In order for paths to match those rules, we add a path separator here
		return !ignoreExpression.IsMatch(EnsureTrailingDirectorySeparator(relativeDirName));
	}

	private HashSet<string> RelevantRelativeFilesBeneathDirectory(string path, CancellationToken cancellationToken)
	{
		return new HashSet<string>(Directory.GetFiles(path, "*", SearchOption.AllDirectories)
			.TakeWhile((s) => !cancellationToken.IsCancellationRequested)
			.Select(p => PXR_PathHelper.MakeRelativePath(path, p)).Where(RelativeFilePathIsRelevant));
	}

	private HashSet<string> RelevantRelativeDirectoriesBeneathDirectory(string path,
		CancellationToken cancellationToken)
	{
		return new HashSet<string>(Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
			.TakeWhile((s) => !cancellationToken.IsCancellationRequested)
			.Select(p => PXR_PathHelper.MakeRelativePath(path, p)).Where(RelativeDirectoryPathIsRelevant));
	}

	public SyncResult Synchronize()
	{
		return Synchronize(CancellationToken.None);
	}

	private void DeleteOutdatedFilesFromTarget(SyncResult syncResult, CancellationToken cancellationToken)
	{
		var outdatedFiles = syncResult.updated.Union(syncResult.deleted);
		foreach (var fileName in outdatedFiles)
		{
			File.Delete(Path.Combine(target, fileName));
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	[SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Local")]
	private void DeleteOutdatedEmptyDirectoriesFromTarget(HashSet<string> sourceDirs, HashSet<string> targetDirs,
		CancellationToken cancellationToken)
	{
		var deleted = targetDirs.Except(sourceDirs).OrderByDescending(s => s);

		// By sorting in descending order above, we delete leaf-first,
		// this is simpler than collapsing the list above (which would also allow us to run these ops in parallel).
		// Assumption is that there are few empty folders to delete
		foreach (var dir in deleted)
		{
			Directory.Delete(Path.Combine(target, dir));
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	[SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Local")]
	private void CreateRelevantDirectoriesAtTarget(HashSet<string> sourceDirs, HashSet<string> targetDirs,
		CancellationToken cancellationToken)
	{
		var created = sourceDirs.Except(targetDirs);
		foreach (var dir in created)
		{
			Directory.CreateDirectory(Path.Combine(target, dir));
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	private void MoveRelevantFilesToTarget(SyncResult syncResult, CancellationToken cancellationToken)
	{
		// step 3: we move all new files to target
		var newFiles = syncResult.created.Union(syncResult.updated);
		foreach (var fileName in newFiles)
		{
			var sourceFileName = Path.Combine(source, fileName);
			var destFileName = Path.Combine(target, fileName);
			// target directory exists due to step CreateRelevantDirectoriesAtTarget()
			File.Move(sourceFileName, destFileName);
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	public SyncResult Synchronize(CancellationToken cancellationToken)
	{
		var sourceDirs = RelevantRelativeDirectoriesBeneathDirectory(source, cancellationToken);
		var targetDirs = RelevantRelativeDirectoriesBeneathDirectory(target, cancellationToken);
		var sourceFiles = RelevantRelativeFilesBeneathDirectory(source, cancellationToken);
		var targetFiles = RelevantRelativeFilesBeneathDirectory(target, cancellationToken);

		var created = sourceFiles.Except(targetFiles).OrderBy(s => s).ToList();
		var updated = sourceFiles.Intersect(targetFiles).OrderBy(s => s).ToList();
		var deleted = targetFiles.Except(sourceFiles).OrderBy(s => s).ToList();
		var syncResult = new SyncResult(created, updated, deleted);

		if (willPerformOperations != null)
		{
			willPerformOperations.Invoke(syncResult);
		}

		DeleteOutdatedFilesFromTarget(syncResult, cancellationToken);
		DeleteOutdatedEmptyDirectoriesFromTarget(sourceDirs, targetDirs, cancellationToken);
		CreateRelevantDirectoriesAtTarget(sourceDirs, targetDirs, cancellationToken);
		MoveRelevantFilesToTarget(syncResult, cancellationToken);

		return syncResult;
	}
}
