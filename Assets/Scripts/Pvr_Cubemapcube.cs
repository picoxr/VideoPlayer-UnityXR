using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Pvr_Cubemapcube : MonoBehaviour
{
    public enum Layout
    {
        Transform32,   
        Capture,    
    }

    private Mesh _mesh;
    protected MeshRenderer _renderer;

    [SerializeField]
    protected Material _material = null;

    [SerializeField]

    private VideoPlayer _videoPlayer = null;

    [SerializeField]
    private float expansion_coeff = 1.01f;

    [SerializeField]
    private Layout _layout = Layout.Transform32;

    public  Texture _texture;
    private bool _verticalFlip;
    private int _textureWidth;
    private int _textureHeight;
    private static int _propApplyGamma;

    private static int _propUseYpCbCr;
    private const string PropChromaTexName = "_ChromaTex";
    private static int _propChromaTex;

    public FilterMode m_FilterMode = FilterMode.Bilinear;

    public TextureWrapMode m_WrapMode = TextureWrapMode.Clamp;

    public VideoPlayer Player
    {
        set { _videoPlayer = value; }
        get { return _videoPlayer; }
    }


    void Awake()
    {
        if (_propApplyGamma == 0)
        {
            _propApplyGamma = Shader.PropertyToID("_ApplyGamma");
        }
        if (_propUseYpCbCr == 0)
            _propUseYpCbCr = Shader.PropertyToID("_UseYpCbCr");
        if (_propChromaTex == 0)
            _propChromaTex = Shader.PropertyToID(PropChromaTexName);
    }

    void Start()
    {
        //Debug.Log("SDK"+Pvr_UnitySDKAPI.System.UPvr_GetUnitySDKVersion());
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.MarkDynamic();
            MeshFilter filter = this.GetComponent<MeshFilter>();
            if (filter != null)
            {
                filter.mesh = _mesh;
            }
            _renderer = this.GetComponent<MeshRenderer>();
            if (_renderer != null)
            {
                _renderer.material = _material;
            }
            CreatMesh();
        }
      
    }

    void OnDestroy()
    {
        if (_mesh != null)
        {
            MeshFilter filter = this.GetComponent<MeshFilter>();
            if (filter != null)
            {
                filter.mesh = null;
            }

#if UNITY_EDITOR
            Mesh.DestroyImmediate(_mesh);
#else
				Mesh.Destroy(_mesh);
#endif
            _mesh = null;
        }

        if (_renderer != null)
        {
            _renderer.material = null;
            _renderer = null;
        }
    }

    void LateUpdate()
    {
        if (Application.isPlaying)
        {
            Texture texture = null;
            bool requiresVerticalFlip = false;
            if (_videoPlayer != null)
          
            {

                texture = _videoPlayer.texture;  
              
                if (_texture != texture ||
                     _verticalFlip != requiresVerticalFlip ||
                    (texture != null && (_textureWidth != texture.width || _textureHeight != texture.height))
                    )
                {
                    _texture = texture;
                    if (texture != null)
                    {
                       
                        UpdateUV(texture.width, texture.height, requiresVerticalFlip);
                    }
                  

                    _renderer.material.mainTexture = _texture;
                }
                else
                {
                    _renderer.material.mainTexture = null;
                }
            }
        }
    }
            private void CreatMesh()
    {
        Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3[] v = new Vector3[]
        {
				// Left
				new Vector3(0f,-1f,0f) - offset,
                new Vector3(0f,0f,0f) - offset,
                new Vector3(0f,0f,-1f) - offset,
                new Vector3(0f,-1f,-1f) - offset,
				// Front
				new Vector3(0f,0f,0f) - offset,
                new Vector3(-1f,0f,0f) - offset,
                new Vector3(-1f,0f,-1f) - offset,
                new Vector3(0f,0f,-1f) - offset,
				// Right
				new Vector3(-1f,0f,0f) - offset,
                new Vector3(-1f,-1f,0f) - offset,
                new Vector3(-1f,-1f,-1f) - offset,
                new Vector3(-1f,0f,-1f) - offset,
				// Back
				new Vector3(-1f,-1f,0f) - offset,
                new Vector3(0f,-1f,0f) - offset,
                new Vector3(0f,-1f,-1f) - offset,
                new Vector3(-1f,-1f,-1f) - offset,
				// Bottom
				new Vector3(0f,-1f,-1f) - offset,
                new Vector3(0f,0f,-1f) - offset,
                new Vector3(-1f,0f,-1f) - offset,
                new Vector3(-1f,-1f,-1f) - offset,
				// Top
				new Vector3(-1f,-1f,0f) - offset,
                new Vector3(-1f,0f,0f) - offset,
                new Vector3(0f,0f,0f) - offset,
                new Vector3(0f,-1f,0f) - offset,


        };

        Matrix4x4 rot = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(-90f, Vector3.right), Vector3.one);
        for (int i = 0; i < v.Length; i++)
        {
            v[i] = rot.MultiplyPoint(v[i]);
        }

        _mesh.vertices = v;

        _mesh.triangles = new int[]  
       
        {
                0,1,2,
                0,2,3,
                4,5,6,
                4,6,7,
                8,9,10,
                8,10,11,
                12,13,14,
                12,14,15,
                16,17,18,
                16,18,19,
                20,21,22,
                20,22,23,


        };

        _mesh.normals = new Vector3[]
        {
				// Left
				new Vector3(-1f,0f,0f),
                new Vector3(-1f,0f,0f),
                new Vector3(-1f,0f,0f),
                new Vector3(-1f,0f,0f),
				// Front
				new Vector3(0f,-1f,0f),
                new Vector3(0f,-1f,0f),
                new Vector3(0f,-1f,0f),
                new Vector3(0f,-1f,0f),
				// Right
				new Vector3(1f,0f,0f),
                new Vector3(1f,0f,0f),
                new Vector3(1f,0f,0f),
                new Vector3(1f,0f,0f),
				// Back
				new Vector3(0f,1f,0f),
                new Vector3(0f,1f,0f),
                new Vector3(0f,1f,0f),
                new Vector3(0f,1f,0f),
				// Bottom
				new Vector3(0f,0f,1f),
                new Vector3(0f,0f,1f),
                new Vector3(0f,0f,1f),
                new Vector3(0f,0f,1f),
				// Top
				new Vector3(0f,0f,-1f),
                new Vector3(0f,0f,-1f),
                new Vector3(0f,0f,-1f),
                new Vector3(0f,0f,-1f)


        };

        UpdateUV(512, 512, false);
    }

    private void UpdateUV(int textureWidth, int textureHeight, bool flipY)
    {
        _textureWidth = textureWidth;
        _textureHeight = textureHeight;
        _verticalFlip = flipY;

        float texWidth = textureWidth;
        float texHeight = textureHeight;

        float blockWidth = texWidth / 3f;
        float pixelOffset = Mathf.Floor(((expansion_coeff * blockWidth) - blockWidth) / 2f);

        float wO = pixelOffset / texWidth;
        float hO = pixelOffset / texHeight;

        const float third = 1f / 3f;
        const float half = 0.5f;

        Vector2[] uv = null;
        if (_layout == Layout.Capture)
        {
            uv = new Vector2[]
            {
					//front (texture middle top) correct left
					new Vector2(third+wO, half-hO),
                    new Vector2((third*2f)-wO, half-hO),
                    new Vector2((third*2f)-wO, 0f+hO),
                    new Vector2(third+wO, 0f+hO),

				
					//left (texture middle bottom) correct front
					new Vector2(third+wO,1f-hO),
                    new Vector2((third*2f)-wO, 1f-hO),
                    new Vector2((third*2f)-wO, half+hO),
                    new Vector2(third+wO, half+hO),

					//bottom (texture left top) correct right
					new Vector2(0f+wO, half-hO),
                    new Vector2(third-wO, half-hO),
                    new Vector2(third-wO, 0f+hO),
                    new Vector2(0f+wO, 0f+hO),

					//top (texture right top) correct rear
					new Vector2((third*2f)+wO, 1f-hO),
                    new Vector2(1f-wO, 1f-hO),
                    new Vector2(1f-wO, half+hO),
                    new Vector2((third*2f)+wO, half+hO),

					//back (texture right bottom) correct ground
					new Vector2((third*2f)+wO, 0f+hO),
                    new Vector2((third*2f)+wO, half-hO),
                    new Vector2(1f-wO, half-hO),
                    new Vector2(1f-wO, 0f+hO),

					//right (texture left bottom) correct sky
					new Vector2(third-wO, 1f-hO),
                    new Vector2(third-wO, half+hO),
                    new Vector2(0f+wO, half+hO),
                    new Vector2(0f+wO, 1f-hO),

            };
        }
        else if (_layout == Layout.Transform32)
        {
            uv = new Vector2[]
            {
					//left
					new Vector2(third+wO,1f-hO),
                    new Vector2((third*2f)-wO, 1f-hO),
                    new Vector2((third*2f)-wO, half+hO),
                    new Vector2(third+wO, half+hO),

					//front
					new Vector2(third+wO, half-hO),
                    new Vector2((third*2f)-wO, half-hO),
                    new Vector2((third*2f)-wO, 0f+hO),
                    new Vector2(third+wO, 0f+hO),

					//right
					new Vector2(0f+wO, 1f-hO),
                    new Vector2(third-wO, 1f-hO),
                    new Vector2(third-wO, half+hO),
                    new Vector2(0f+wO, half+hO),

					//back
					new Vector2((third*2f)+wO, half-hO),
                    new Vector2(1f-wO, half-hO),
                    new Vector2(1f-wO, 0f+hO),
                    new Vector2((third*2f)+wO, 0f+hO),

					//bottom
					new Vector2(0f+wO, 0f+hO),
                    new Vector2(0f+wO, half-hO),
                    new Vector2(third-wO, half-hO),
                    new Vector2(third-wO, 0f+hO),

					//top
					new Vector2(1f-wO, 1f-hO),
                    new Vector2(1f-wO, half+hO),
                    new Vector2((third*2f)+wO, half+hO),
                    new Vector2((third*2f)+wO, 1f-hO)

            };
        }

        else if (_layout == Layout.Transform32)
        {
            uv = new Vector2[]
            {
					//left
					new Vector2(third+wO,1f-hO),
                    new Vector2((third*2f)-wO, 1f-hO),
                    new Vector2((third*2f)-wO, half+hO),
                    new Vector2(third+wO, half+hO),

					//front
					new Vector2(third+wO, half-hO),
                    new Vector2((third*2f)-wO, half-hO),
                    new Vector2((third*2f)-wO, 0f+hO),
                    new Vector2(third+wO, 0f+hO),

					//right
					new Vector2(0f+wO, 1f-hO),
                    new Vector2(third-wO, 1f-hO),
                    new Vector2(third-wO, half+hO),
                    new Vector2(0f+wO, half+hO),

					//back
					new Vector2((third*2f)+wO, half-hO),
                    new Vector2(1f-wO, half-hO),
                    new Vector2(1f-wO, 0f+hO),
                    new Vector2((third*2f)+wO, 0f+hO),

					//bottom
					new Vector2(0f+wO, 0f+hO),
                    new Vector2(0f+wO, half-hO),
                    new Vector2(third-wO, half-hO),
                    new Vector2(third-wO, 0f+hO),

					//top
					new Vector2(1f-wO, 1f-hO),
                    new Vector2(1f-wO, half+hO),
                    new Vector2((third*2f)+wO, half+hO),
                    new Vector2((third*2f)+wO, 1f-hO)

            };
        }

        if (flipY)
        {
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i].y = 1f - uv[i].y;
            }
        }

        _mesh.uv = uv;
        _mesh.UploadMeshData(false);
    }
}
