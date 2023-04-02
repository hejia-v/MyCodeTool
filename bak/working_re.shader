#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_explicit_uniform_location : require

// 定义uniform变量
#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif

// 定义uniform变量
layout(std140) uniform VGlobals {
	vec4 unused_0_0[14];
	vec4 _ProjectionParams;
	vec4 unused_0_2[9];
	mat4x4 unity_CameraProjection;
	vec4 unused_0_4[23];
	mat4x4 glstate_matrix_projection;
	vec4 unused_0_6[4];
	mat4x4 unity_MatrixInvV;
	vec4 unused_0_8[31];
	mat4x4 _NonJitteredProjMatrix;
	vec4 unused_0_10[51];
	vec4 _DissolvePosMaskPos;
	float _DissolvePosMaskWorldON;
	vec3 _DissolvePosMaskRootOffset;
	float _DissolvePosMaskFilpOn;
	float _DissolvePosMaskOn;
	float _DissolvePosMaskGlobalOn;
	vec4 unused_0_17[67];
	vec4 _ES_EffCustomLightPosition;
	vec4 unused_0_19[36];
	float _OutlineScale;
	vec4 unused_0_21[10];
};
layout(std140) uniform UnityPerDraw {
	mat4x4 unity_ObjectToWorld;
	mat4x4 unity_WorldToObject;
	mat4x4 unity_MatrixMV;
	vec4 unused_1_3[30];
};
layout(std140) uniform CharacterOutlineBuffer {
	vec4 unused_2_0[10];
	float _OutlineWidth;
	float _OneMinusCharacterOutlineWidthScale;
	vec4 unused_2_3[8];
	float _DissolveUV;
	vec4 _DissolveST;
	vec4 _DistortionST;
	vec4 unused_2_7[5];
};
layout(std140) uniform UnityPerMaterial {
	vec4 unused_3_0[40];
	float _UsingDitherAlpha;
	float _DitherAlpha;
	vec4 unused_3_3[5];
};

// 定义输入输出变量
in  vec4 in_POSITION0;
in  vec4 in_TANGENT0;
in  vec4 in_TEXCOORD0;
in  vec4 in_TEXCOORD2;
in  vec4 in_COLOR0;
out vec4 vs_TEXCOORD0;
out vec4 vs_TEXCOORD1;
out vec3 vs_TEXCOORD6;
out vec4 vs_TEXCOORD2;
out vec4 vs_TEXCOORD4;
out vec4 vs_TEXCOORD5;

// 定义变量
vec4 u_xlat0;
bool u_xlatb0;
vec4 u_xlat1;
vec4 u_xlat2;
vec3 u_xlat3;
vec3 u_xlat4;
float u_xlat12;
bool u_xlatb12;
float u_xlat13;

// 写python工具重构变量

// 主函数
void main()
{
    // 计算u_xlat0
    u_xlat0.xyz = unity_MatrixInvV[0].yyy * unity_WorldToObject[1].xyz;
    u_xlat0.xyz = unity_WorldToObject[0].xyz * unity_MatrixInvV[0].xxx + u_xlat0.xyz;
    u_xlat0.xyz = unity_WorldToObject[2].xyz * unity_MatrixInvV[0].zzz + u_xlat0.xyz;
    u_xlat0.xyz = unity_WorldToObject[3].xyz * unity_MatrixInvV[0].www + u_xlat0.xyz;
    u_xlat0.x = dot(u_xlat0.xyz, in_TANGENT0.xyz);

    // 计算u_xlat1
    u_xlat1.xyz = unity_MatrixInvV[1].yyy * unity_WorldToObject[1].xyz;
    u_xlat1.xyz = unity_WorldToObject[0].xyz * unity_MatrixInvV[1].xxx + u_xlat1.xyz;
    u_xlat1.xyz = unity_WorldToObject[2].xyz * unity_MatrixInvV[1].zzz + u_xlat1.xyz;
    u_xlat1.xyz = unity_WorldToObject[3].xyz * unity_MatrixInvV[1].www + u_xlat1.xyz;
    u_xlat0.y = dot(u_xlat1.xyz, in_TANGENT0.xyz);
    u_xlat0.z = -0.100000001;
    u_xlat12 = dot(u_xlat0.xyz, u_xlat0.xyz);
    u_xlat12 = inversesqrt(u_xlat12);
    u_xlat0.xyz = vec3(u_xlat12) * u_xlat0.xyz;

    // 计算u_xlat1
    u_xlat1.xyz = in_POSITION0.yyy * unity_MatrixMV[1].xyz;
    u_xlat1.xyz = unity_MatrixMV[0].xyz * in_POSITION0.xxx + u_xlat1.xyz;
    u_xlat1.xyz = unity_MatrixMV[2].xyz * in_POSITION0.zzz + u_xlat1.xyz;
    u_xlat1.xyz = u_xlat1.xyz + unity_MatrixMV[3].xyz;

    u_xlat12 = u_xlat1.z / unity_CameraProjection[1].y;
    u_xlat12 = abs(u_xlat12) / _OutlineScale;
    u_xlat12 = inversesqrt(u_xlat12);
    u_xlat12 = float(1.0) / u_xlat12;
    u_xlat13 = _OutlineScale * _OutlineWidth;
    u_xlat13 = u_xlat13 * in_COLOR0.w;
    u_xlat12 = u_xlat12 * u_xlat13;
    u_xlat13 = (-_OneMinusCharacterOutlineWidthScale) + 1.0;
    u_xlat12 = u_xlat12 * u_xlat13;
    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat12) + u_xlat1.xyz;

    // 计算u_xlat1
    u_xlat1 = u_xlat0.yyyy * _NonJitteredProjMatrix[1];
    u_xlat1 = _NonJitteredProjMatrix[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = _NonJitteredProjMatrix[2] * u_xlat0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + _NonJitteredProjMatrix[3];
    u_xlat2.x = glstate_matrix_projection[0].z;
    u_xlat2.y = glstate_matrix_projection[1].z;
    u_xlat2.z = glstate_matrix_projection[2].z;
    u_xlat2.w = glstate_matrix_projection[3].z;
    u_xlat0.w = 1.0;
    u_xlat2.z = dot(u_xlat2, u_xlat0);
    u_xlat3.x = glstate_matrix_projection[0].x;
    u_xlat3.y = glstate_matrix_projection[2].x;
    u_xlat3.z = glstate_matrix_projection[3].x;
    u_xlat2.x = dot(u_xlat3.xyz, u_xlat0.xzw);
    u_xlat3.x = glstate_matrix_projection[1].y;
    u_xlat3.y = glstate_matrix_projection[2].y;
    u_xlat3.z = glstate_matrix_projection[3].y;
    u_xlat2.y = dot(u_xlat3.xyz, u_xlat0.yzw);
    u_xlat0.x = glstate_matrix_projection[2].w;
    u_xlat0.y = glstate_matrix_projection[3].w;
    u_xlat2.w = dot(u_xlat0.xy, u_xlat0.zw);

    u_xlatb0 = 0.0<_UsingDitherAlpha;
    positionCSFinal = (bool(u_xlatb0)) ? u_xlat1 : u_xlat2;
    gl_Position = positionCSFinal;

    // 输出变量
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    vs_TEXCOORD0.zw = vec2(0.0, 0.0);
    vs_TEXCOORD1.xw = in_COLOR0.wx;
    vs_TEXCOORD1.yz = vec2(1.0, 1.0);

    u_xlat4.xyz = in_POSITION0.yyy * unity_ObjectToWorld[1].xyz;
    u_xlat4.xyz = unity_ObjectToWorld[0].xyz * in_POSITION0.xxx + u_xlat4.xyz;
    u_xlat4.xyz = unity_ObjectToWorld[2].xyz * in_POSITION0.zzz + u_xlat4.xyz;
    positionWS.xyz = u_xlat4.xyz + unity_ObjectToWorld[3].xyz;
    vs_TEXCOORD6.xyz = positionWS.xyz; // positionWS

    positionWS.xyz = positionWS.xyz - _DissolvePosMaskPos.xyz - in_POSITION0.xyz;
    positionWS.xyz = _DissolvePosMaskWorldON * positionWS.xyz + in_POSITION0.xyz;
    // 将y分量乘以_ProjectionParams.x的作用是将y坐标从[-1, 1]的范围缩放到[0, 1]的范围内，这个操作通常被称为"normalized device y" 或 "homogeneous clip y"。
    // 这样做可以使得接下来的插值计算更加精确，并且可以避免出现深度测试错误的情况。
    positionCSFinal.y = positionCSFinal.y * _ProjectionParams.x;
    u_xlat2.xzw = positionCSFinal.xwy * vec3(0.5, 0.5, 0.5);
    positionCSFinal.xy = u_xlat2.zz + u_xlat2.xw;

    vs_TEXCOORD2.xyw = bool(u_xlatb0) ? positionCSFinal.xyw : vec3(0.0, 0.0, 0.0);
    vs_TEXCOORD2.z = u_xlatb0 ? _DitherAlpha : float(0.0);

    dissolveUV.xy = _DissolveUV * (-in_TEXCOORD0.xy + in_TEXCOORD2.xy) + in_TEXCOORD0.xy;

    vs_TEXCOORD4.xy = dissolveUV.xy * _DissolveST.xy + _DissolveST.zw;
    vs_TEXCOORD4.zw = dissolveUV.xy * _DistortionST.xy + _DistortionST.zw;


    u_xlat0.xyz = _DissolvePosMaskGlobalOn * (-positionWS.xyz + _ES_EffCustomLightPosition.xyz) + positionWS.xyz;
    u_xlat0.xyz = u_xlat0.xyz - _DissolvePosMaskRootOffset.xyz;
    lightDir.xyz = _ES_EffCustomLightPosition.xyz + (-unity_ObjectToWorld[3].xyz);
    u_xlat3.xyz = _DissolvePosMaskWorldON * (-unity_ObjectToWorld[3].xyz) + _DissolvePosMaskPos.xyz;
    lightDir.xyz = lightDir.xyz + -u_xlat3.xyz;
    lightDir.xyz = _DissolvePosMaskGlobalOn * lightDir.xyz + u_xlat3.xyz;

    u_xlat12 = dot(lightDir.xyz, lightDir.xyz);
    u_xlat12 = inversesqrt(u_xlat12);
    u_xlat3.xyz = vec3(u_xlat12) * lightDir.xyz;

    u_xlat12 = dot(abs(lightDir.xyz), vec3(1.0, 1.0, 1.0));
    u_xlatb12 = u_xlat12>=0.00100000005;
    u_xlat0.x = dot(u_xlat3.xyz, u_xlat0.xyz);
    positionWS.x = max(_DissolvePosMaskPos.w, 0.00999999978);
    u_xlat0.x = positionWS.x + abs(u_xlat0.x);
    positionWS.x = positionWS.x + positionWS.x;
    u_xlat0.x = u_xlat0.x / positionWS.x;
    positionWS.x = u_xlat0.x * -2.0 + 1.0;
    u_xlat0.x = _DissolvePosMaskFilpOn * positionWS.x + u_xlat0.x;
    u_xlat0.x = u_xlat0.x + -_DissolvePosMaskOn;
    u_xlat0.x = u_xlat0.x + 1.0;
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
    dissolveUV.z = (u_xlatb12) ? u_xlat0.x : 1.0;
    dissolveUV.w = 0.0;
    vs_TEXCOORD5 = dissolveUV.xzww;
    return;
}