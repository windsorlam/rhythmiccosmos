// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/Curved World/One Directional Light/Terrain/2Tex Bumped Specular"
{
	Properties    
	{          
		//Tag               
		[Tag]     
		V_CW_TAG("", float) = 0      
		     
		//Default Options 
		[DefaultOptions]      
		V_CW_D_OPTIONS("", float) =  0   
		
		[HideInInspector]  
		_Color("Main Color", color) = (1, 1, 1, 1)  
		[HideInInspector]
		_MainTex ("Base (RGB)", 2D) = "white" {}

		 
		 
		_V_CW_Specular_Intensity("Specular Intensity", Range(0, 5)) = 1
		[NoUVTexture]
		_V_CW_Specular_Lookup("Specular Lookup", 2D) = "black"{}

		[KeywordEnum(Texture, VertexColor)] V_CW_TERRAINBLEND ("Blend By", Float) = 0

		[CanBeHidden]
		_V_CW_Control ("Control (RGBA)", 2D) = "gray" {}
		_V_CW_Splat1_uvScale("Layer 1 UV Scale", float) = 1
		[NoUVTexture]
		_V_CW_Splat1 ("Layer 1 (R)", 2D) = "black" {}
		[NoUVTexture]
		_V_CW_Splat1_Bump ("Layer 1 Normalmap", 2D) = "bump" {}
		_V_CW_Splat2_uvScale("Layer 2 UV Scale", float) = 1
		[NoUVTexture]
		_V_CW_Splat2 ("Layer 2 (G)", 2D) = "black" {}
		[NoUVTexture]
		_V_CW_Splat2_Bump ("Layer 2 Normalmap", 2D) = "bump" {}
		 
		//CurvedWorld Options
		[CurvedWorldOptions] 
		V_CW_W_OPTIONS("", float) = 0 
		 

		[HideInInspector]
		_V_CW_Z_Bend_Size("", float) = 0
		[HideInInspector]
		_V_CW_Z_Bend_Bias("", float) = 0
		[HideInInspector]
		_V_CW_Y_Bend_Size("", float) = 0
		[HideInInspector]
		_V_CW_X_Bend_Size("", float) = 0
		[HideInInspector]
		_V_CW_Camera_Bend_Offset("", float) = 0
		 

		[HideInInspector]
		_V_CW_Rim_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Rim_Bias("", Range(-1, 1)) = 0.2
		   
		[HideInInspector]  
		_V_CW_Fog_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Fog_Density("", Range(0.0, 1.0)) = 1 
		[HideInInspector] 
		_V_CW_Fog_Start("", float) = 0
		[HideInInspector]
		_V_CW_Fog_End("", float) = 100  
		
		[HideInInspector]
		_V_CW_IBL_Intensity("", float) = 1
		[HideInInspector] 
		_V_CW_IBL_Contrast("", float) = 1 
		[HideInInspector]   
		_V_CW_IBL_Cube("", cube ) = ""{}  

		[HideInInspector]
		_V_CW_Emission_Color("", color) = (1, 1, 1, 1)
		[HideInInspector]
		_V_CW_Emission_Strength("", float) = 1
	}
	 
	SubShader 
	{
		Tags { "RenderType"="CurvedWorld_Local_Opaque" 
		       "CurvedWorldTag"="One Directional Light/Terrain/2Tex Bumped Specular" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 200
		
		Fog{Mode Off} 

		Pass  
	    { 
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" } 

			CGPROGRAM      
			#pragma vertex vert 
	    	#pragma fragment frag  
	    	#pragma fragmentoption ARB_precision_hint_fastest

			#define UNITY_PASS_FORWARDBASE  
			   
			#pragma multi_compile V_CW_TERRAINBLEND_TEXTURE V_CW_TERRAINBLEND_VERTEXCOLOR

			#pragma multi_compile V_CW_UNITY_VERTEXLIGHT_OFF V_CW_UNITY_VERTEXLIGHT_ON
			#pragma multi_compile V_CW_FOG_OFF V_CW_FOG_ON
			#pragma multi_compile V_CW_IBL_OFF V_CW_IBL_ON
			  
			#define V_CW_SPECULAR
			#define V_CW_LIGHT_PER_PIXEL
			#define V_CW_TERRAIN			
			#define V_CW_TERRAIN_2TEX 
			#define V_CW_BUMP 
			     
			    
			#ifdef V_CW_UNITY_VERTEXLIGHT_ON    
				#pragma multi_compile_fwdbase nodirlightmap  
			#else 
				#pragma multi_compile_fwdbase nodirlightmap novertexlight
			#endif

			#pragma exclude_renderers d3d11_9x
			#include "UnityCG.cginc"  
            #include "AutoLight.cginc"
			   
			   
			#include "../cginc/CurvedWorld.cginc"
			   
			ENDCG   
			 
		}	//Pass   		

		//ShadowCaster
		UsePass "Hidden/VacuumShaders/Curved World/ShadowPass/SHADOWCASTER"

		//ShadowCollector
		UsePass "Hidden/VacuumShaders/Curved World/ShadowPass/SHADOWCOLLECTOR"
		
	}	//SubShader

	

	//Fallback - Unlit
	SubShader 
	{
		Tags { "RenderType"="CurvedWorld_Local_Opaque" 
		       "CurvedWorldTag"="Unlit/Terrain/2Tex" 
			   "CurvedWorldBakedKeywords"="" 
			 }
		LOD 150
		
		Fog{Mode Off}

		Pass
	    {
			CGPROGRAM
			#pragma vertex vert
	    	#pragma fragment frag
	    	#pragma fragmentoption ARB_precision_hint_fastest
			#define UNITY_PASS_UNLIT

			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF

			#pragma multi_compile V_CW_TERRAINBLEND_TEXTURE V_CW_TERRAINBLEND_VERTEXCOLOR

			#pragma multi_compile V_CW_FOG_OFF V_CW_FOG_ON
			#pragma multi_compile V_CW_IBL_OFF V_CW_IBL_ON
			
			#define V_CW_TERRAIN
			#define V_CW_TERRAIN_2TEX


			#pragma exclude_renderers d3d11_9x
			#include "../cginc/CurvedWorld.cginc" 

			ENDCG

		}	//Pass
	}	//SubShader
	 
	
	CustomEditor "CurvedWorldMaterial_Editor"

}	//Shader
