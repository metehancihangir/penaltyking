Shader "PenaltyKing/PlayerKit"
{
 Properties {
  [PerRendererData] _MainTex("Sprite",2D)="white"{}
  _Color("Tint",Color)=(1,1,1,1)
  _Kit("Kit",Float)=0
  _Keeper("Keeper",Float)=0
  _Chroma("Chroma sprite",Float)=0
  _SpriteUV("Sprite UV",Vector)=(0,0,1,1)
  _StencilComp("Stencil Comparison",Float)=8
  _Stencil("Stencil ID",Float)=0
  _StencilOp("Stencil Operation",Float)=0
  _StencilWriteMask("Stencil Write Mask",Float)=255
  _StencilReadMask("Stencil Read Mask",Float)=255
  _ColorMask("Color Mask",Float)=15
 }
 SubShader {
  Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True"}
  Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
  Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
   #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
   #include "UnityCG.cginc"
   #include "UnityUI.cginc"
   struct appdata {float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
   struct v2f {float4 vertex:SV_POSITION;fixed4 color:COLOR;float2 uv:TEXCOORD0;float4 local:TEXCOORD1;};
   sampler2D _MainTex;fixed4 _Color,_TextureSampleAdd;float4 _ClipRect,_SpriteUV;float _Kit,_Keeper,_Chroma;
   v2f vert(appdata v){v2f o;o.local=v.vertex;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
   fixed4 frag(v2f i):SV_Target {
    fixed4 c=(tex2D(_MainTex,i.uv)+_TextureSampleAdd)*i.color;
    float3 key=c.rgb;
    #ifndef UNITY_COLORSPACE_GAMMA
    key=LinearToGammaSpace(key);
    #endif
    float r=key.r,g=key.g,b=key.b;
    if (_Chroma>.5 && r>.45 && b>.35 && g<min(r,b)*.6) discard;
    float localY=(i.uv.y-_SpriteUV.y)/max(.001,_SpriteUV.w-_SpriteUV.y);
    bool redFabric = _Chroma>.5 ? (r>g*3.2 && r>b*2 && r>.18) : (localY<.94 && r>g*2.1 && r>b*1.5 && b>g*.65);
    if(_Kit>.5 && _Kit<1.5 && redFabric) key=float3(.42,.78,.98)*r;
    if(_Kit>1.5 && g>r*.40 && g<r*1.12 && b<r*.20) key=float3(.22,.88,.38)*max(r,g);
    if(_Keeper>.5 && r>g*1.15 && r<g*1.9 && g>b*1.15 && b>r*.22) key=lerp(key,sqrt(max(key,0)),.16);
    #ifndef UNITY_COLORSPACE_GAMMA
    key=GammaToLinearSpace(key);
    #endif
    c.rgb=key;
    #ifdef UNITY_UI_CLIP_RECT
    c.a*=UnityGet2DClipping(i.local.xy,_ClipRect);
    #endif
    #ifdef UNITY_UI_ALPHACLIP
    clip(c.a-.001);
    #endif
    return c;
   }
   ENDCG
  }
 }
}
