/**

Copyright (c) 2013 DFKI - German Research Center for Artificial Intelligence
                   http://www.dfki.de/web/forschung/asr

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

**/

XML3D.shaders.register("envlight", {

    vertex : [
        "attribute vec3 position;",
        "attribute vec3 normal;",
        "attribute vec3 color;",
        "attribute vec2 texcoord;",

        "varying vec3 fragNormal;",
        "varying vec3 fragVertexPosition;",
        "varying vec3 fragEyeVector;",
        "varying vec2 fragTexCoord;",
        "varying vec3 fragVertexColor;",

        "uniform mat4 modelViewProjectionMatrix;",
        "uniform mat4 modelViewMatrix;",
        "uniform mat3 normalMatrix;",
        "uniform vec3 eyePosition;",

        "void main(void) {",
        "    vec3 pos = position;",
        "    vec3 norm = normal;",

        "    gl_Position = modelViewProjectionMatrix * vec4(pos, 1.0);",
        "    fragNormal = normalize(normalMatrix * norm);",
        "    fragVertexPosition = (modelViewMatrix * vec4(pos, 1.0)).xyz;",
        "    fragEyeVector = normalize(fragVertexPosition);",
        "    fragTexCoord = texcoord;",
        "    fragVertexColor = color;",
        "}"
    ].join("\n"),

    fragment : [
        "uniform float ambientIntensity;",
        "uniform vec3 diffuseColor;",
        "uniform vec3 emissiveColor;",
        "uniform float shininess;",
        "uniform vec3 specularColor;",
        "uniform float transparency;",
        "uniform mat4 viewMatrix;",
        "uniform bool useVertexColor;",

        "#if HAS_EMISSIVETEXTURE",
        "uniform sampler2D emissiveTexture;",
        "#endif",
        "#if HAS_DIFFUSETEXTURE",
        "uniform sampler2D diffuseTexture;",
        "#endif",
        "#if HAS_SPECULARTEXTURE",
        "uniform sampler2D specularTexture;",
        "#endif",

        "uniform sampler2D spheremap;",

        "varying vec3 fragNormal;",
        "varying vec3 fragVertexPosition;",
        "varying vec3 fragEyeVector;",
        "varying vec2 fragTexCoord;",
        "varying vec3 fragVertexColor;",

        "#if MAX_POINTLIGHTS > 0",
        "uniform vec3 pointLightAttenuation[MAX_POINTLIGHTS];",
        "uniform vec3 pointLightPosition[MAX_POINTLIGHTS];",
        "uniform vec3 pointLightIntensity[MAX_POINTLIGHTS];",
        "uniform bool pointLightOn[MAX_POINTLIGHTS];",
        "#endif",

        "#if MAX_DIRECTIONALLIGHTS > 0",
        "uniform vec3 directionalLightDirection[MAX_DIRECTIONALLIGHTS];",
        "uniform vec3 directionalLightIntensity[MAX_DIRECTIONALLIGHTS];",
        "uniform bool directionalLightOn[MAX_DIRECTIONALLIGHTS];",
        "#endif",

        "#if MAX_SPOTLIGHTS > 0",
        "uniform vec3 spotLightAttenuation[MAX_SPOTLIGHTS];",
        "uniform vec3 spotLightPosition[MAX_SPOTLIGHTS];",
        "uniform vec3 spotLightIntensity[MAX_SPOTLIGHTS];",
        "uniform bool spotLightOn[MAX_SPOTLIGHTS];",
        "uniform vec3 spotLightDirection[MAX_SPOTLIGHTS];",
        "uniform float spotLightCosFalloffAngle[MAX_SPOTLIGHTS];",
        "uniform float spotLightCosSoftFalloffAngle[MAX_SPOTLIGHTS];",
        "uniform float spotLightSoftness[MAX_SPOTLIGHTS];",
        "#endif",

        "void main(void) {",
        "  float alpha =  max(0.0, 1.0 - transparency);",
        "  vec3 objDiffuse = diffuseColor;",
        "  if(useVertexColor)",
        "    objDiffuse *= fragVertexColor;",
        "  #if HAS_DIFFUSETEXTURE",
        "    vec4 texDiffuse = texture2D(diffuseTexture, fragTexCoord);",
        "    alpha *= texDiffuse.a;",
        "    objDiffuse *= texDiffuse.rgb;",
        "  #endif",
        "  if (alpha < 0.05) discard;",
        "  #if HAS_EMISSIVETEXTURE",
        "    vec3 color = emissiveColor * texture2D(emissiveTexture, fragTexCoord).rgb + (ambientIntensity * objDiffuse);",
        "  #else",
        "    vec3 color = emissiveColor + (ambientIntensity * objDiffuse);",
        "  #endif",
        "  vec3 objSpecular = specularColor;",
        "  #if HAS_SPECULARTEXTURE",
        "    objSpecular = objSpecular * texture2D(specularTexture, fragTexCoord).rgb;",
        "  #endif",
        "  vec2 uv = 0.45*fragNormal.xy + vec2(0.5);",
		"  vec4 envLight = texture2D(spheremap, uv);",
		"  color = color + envLight.r * objDiffuse;",
        "#if MAX_POINTLIGHTS > 0",
        "  for (int i=0; i<MAX_POINTLIGHTS; i++) {",
        "    if(!pointLightOn[i])",
        "      continue;",
        "    vec4 lPosition = viewMatrix * vec4( pointLightPosition[ i ], 1.0 );",
        "    vec3 L = lPosition.xyz - fragVertexPosition;",
        "    float dist = length(L);",
        "    L = normalize(L);",
        "    vec3 R = normalize(reflect(L,fragNormal));",
        "    float atten = 1.0 / (pointLightAttenuation[i].x + pointLightAttenuation[i].y * dist + pointLightAttenuation[i].z * dist * dist);",
        "    vec3 Idiff = pointLightIntensity[i] * objDiffuse * max(dot(fragNormal,L),0.0);",
        "    vec3 Ispec = pointLightIntensity[i] * objSpecular * pow(max(dot(R,fragEyeVector),0.0), shininess*128.0);",
        "    color = color + (atten*(Idiff + Ispec));",
        "  }",
        "#endif",

        "#if MAX_DIRECTIONALLIGHTS > 0",
        "  for (int i=0; i<MAX_DIRECTIONALLIGHTS; i++) {",
        "    if(!directionalLightOn[i])",
        "      continue;",
        "    vec4 lDirection = viewMatrix * vec4(directionalLightDirection[i], 0.0);",
        "    vec3 L =  normalize(-lDirection.xyz);",
        "    vec3 R = normalize(reflect(L,fragNormal));",
        "    vec3 Idiff = directionalLightIntensity[i] * objDiffuse * max(dot(fragNormal,L),0.0);",
        "    vec3 Ispec = directionalLightIntensity[i] * objSpecular * pow(max(dot(R,fragEyeVector),0.0), shininess*128.0);",
        "    color = color + ((Idiff + Ispec));",
        "  }",
        "#endif",

        "#if MAX_SPOTLIGHTS > 0",
        "  for (int i=0; i<MAX_SPOTLIGHTS; i++) {",
        "    if(!spotLightOn[i])",
        "      continue;",
        "    vec4 lPosition = viewMatrix * vec4( spotLightPosition[ i ], 1.0 );",
        "    vec3 L = lPosition.xyz - fragVertexPosition;",
        "    float dist = length(L);",
        "    L = normalize(L);",
        "    vec3 R = normalize(reflect(L,fragNormal));",
        "    float atten = 1.0 / (spotLightAttenuation[i].x + spotLightAttenuation[i].y * dist + spotLightAttenuation[i].z * dist * dist);",
        "    vec3 Idiff = spotLightIntensity[i] * objDiffuse * max(dot(fragNormal,L),0.0);",
        "    vec3 Ispec = spotLightIntensity[i] * objSpecular * pow(max(dot(R,fragEyeVector),0.0), shininess*128.0);",
        "    vec4 lDirection = viewMatrix * vec4(-spotLightDirection[i], 0.0);",
        "    vec3 D = normalize(lDirection.xyz);",
        "    float angle = dot(L, D);",
        "    if(angle > spotLightCosFalloffAngle[i]) {",
        "       float softness = 1.0;",
        "       if (angle < spotLightCosSoftFalloffAngle[i])",
        "           softness = (angle - spotLightCosFalloffAngle[i]) /  (spotLightCosSoftFalloffAngle[i] -  spotLightCosFalloffAngle[i]);",
        "       color += atten*softness*(Idiff + Ispec);",
        "    }",
        "  }",
        "#endif",


		"  // color = envLight.rgb; // vec3(1.0, 0.0, 0.0);",
		
        "  gl_FragColor = vec4(color, alpha);",
        "}"
    ].join("\n"),

    addDirectives: function(directives, lights, params) {
        var pointLights = lights.point ? lights.point.length : 0;
        var directionalLights = lights.directional ? lights.directional.length : 0;
        var spotLights = lights.spot ? lights.spot.length : 0;
        directives.push("MAX_POINTLIGHTS " + pointLights);
        directives.push("MAX_DIRECTIONALLIGHTS " + directionalLights);
        directives.push("MAX_SPOTLIGHTS " + spotLights);
        directives.push("HAS_DIFFUSETEXTURE " + ('diffuseTexture' in params ? "1" : "0"));
        directives.push("HAS_SPECULARTEXTURE " + ('specularTexture' in params ? "1" : "0"));
        directives.push("HAS_EMISSIVETEXTURE " + ('emissiveTexture' in params ? "1" : "0"));
    },
    hasTransparency: function(params) {
        return params.transparency && params.transparency.getValue()[0] > 0.001;
    },
    uniforms: {
        diffuseColor    : [1.0, 1.0, 1.0],
        emissiveColor   : [0.0, 0.0, 0.0],
        specularColor   : [0.0, 0.0, 0.0],
        transparency    : 0.0,
        shininess       : 0.2,
        ambientIntensity: 0.0,
        useVertexColor : false
    },

    samplers: {
        spheremap : null,
        diffuseTexture : null,
        emissiveTexture : null,
        specularTexture : null
    },

    attributes: {
        normal : {
            required: true
        },
        texcoord: null,
        color: null
    }
});


function texture2D(tex, uv)
{
	var d = tex.data;
	var imgX = uv[0] * tex.width;
	var imgY = uv[1] * tex.height;
	
	if (imgX >= tex.width) return [1.0, 1.0, 1.0];
	if (imgY >= tex.height) return [1.0, 1.0, 1.0];
	
	var nearest = 4 * (Math.floor(imgY) * tex.width + Math.floor(imgX));
	
	// return [ imgX, imgY, 255, 255 ];
	return [ d[nearest], d[nearest+1], d[nearest+2] ];
};


mat3 = XML3D.math.mat3;
mat4 = XML3D.math.mat4;

vec2 = XML3D.math.vec2;
vec3 = XML3D.math.vec3;
vec4 = XML3D.math.vec4;



Xflow.registerOperator("xflmix.lightprobe", {
    outputs: [
		{type: 'texture', name : 'spheremap', customAlloc: true}
	],
    params:  [
		{type: 'texture', source : 'input'},
		{type: 'float4x4', source : 'arBaseTf'},
		{type: 'float3', source : 'lightProbePosition'},
		{type: 'float4x4', source : 'projectionTf'}
	],
    alloc: function(sizes, input, arBaseTf, lightProbePosition, projectionTf) {
        var samplerConfig = new Xflow.SamplerConfig;
        samplerConfig.setDefaults();
        sizes['spheremap'] = {
            imageFormat : {width: 256, height: 256},
            samplerConfig : samplerConfig
        };
    },
    evaluate: function(spheremap, input, arBaseTf, lightProbePosition, projectionTf) {

		var R = 50.0;

		var s = input.data;
        var d = spheremap.data;

	///////////////////////////////
		// for (var i = 0, iEnd = spheremap.height*spheremap.width; i < iEnd; i++) {
			// var destOffset = i * 4;
			// d[destOffset]     = 127;
			// d[destOffset + 1] = 127;
			// d[destOffset + 2] = 127;
			// d[destOffset + 3] = 255;
		// }
		// return true;
	///////////////////////////////

		
		var modelViewMatrix = mat4.create();
		
		// var lpTf = mat4.translate(mat4.create(), mat4.create(), vec3.fromValues(lightProbePosition[0], lightProbePosition[1], lightProbePosition[2]));
		// var modelViewMatrix = arBaseTf; // mat4.multiply(mat4.create(), arBaseTf, lpTf);
		mat4.multiply(modelViewMatrix, modelViewMatrix, arBaseTf);
		mat4.translate(modelViewMatrix, modelViewMatrix, vec3.fromValues(lightProbePosition[0], lightProbePosition[1], lightProbePosition[2]));
		//console.log(modelViewMatrix);
		
		var mvMat3 = mat3.fromMat4(mat3.create(), modelViewMatrix);
		//console.log(mvMat3);
		
		var vR = vec3.fromValues(R, 0.0, 0.0);
		//console.log(vR);
		
		var eyeR = vec3.transformMat3(vec3.create(), vR, mvMat3);
		//console.log(eyeR);
		
		var lR = vec3.length(eyeR);
		//console.log(lR);
		
		var eyeCenter = vec4.transformMat4(vec4.create(), vec4.fromValues(0.0, 0.0, 0.0, 1.0), modelViewMatrix);
		
		// TODO: check why this fixes the wrong y orientation in screen / eye space
		eyeCenter[1] = -eyeCenter[1];
		
		// var projCenter = vec4.transformMat4(vec4.create(), eyeCenter, projectionTf);
		// vec4.scale(projCenter, projCenter, 1.0/projCenter[3]);
		//console.log(eyeCenter);
		//projCenter = vec3.fromValues(projCenter[0]/projCenter[3], projCenter[1]/projCenter[3], projCenter[2]/projCenter[3]);
		
		var corner00 = vec4.add(vec4.create(), eyeCenter, vec4.fromValues(-lR, -lR, 0.0, 0.0));
		var corner10 = vec4.add(vec4.create(), eyeCenter, vec4.fromValues( lR, -lR, 0.0, 0.0));
		var corner01 = vec4.add(vec4.create(), eyeCenter, vec4.fromValues(-lR,  lR, 0.0, 0.0));
		var corner11 = vec4.add(vec4.create(), eyeCenter, vec4.fromValues( lR,  lR, 0.0, 0.0));
		
		vec4.transformMat4(corner00, corner00, projectionTf);
		vec4.scale(corner00, corner00, 0.5/corner00[3]);

		vec4.transformMat4(corner10, corner10, projectionTf);
		vec4.scale(corner10, corner10, 0.5/corner10[3]);

		vec4.transformMat4(corner01, corner01, projectionTf);
		vec4.scale(corner01, corner01, 0.5/corner01[3]);
		
		vec4.transformMat4(corner11, corner11, projectionTf);
		vec4.scale(corner11, corner11, 0.5/corner11[3]);
		
		var c00 = vec2.add(vec2.create(), vec2.fromValues(0.5, 0.5), vec2.fromValues(corner00[0], corner00[1]));
		var c10 = vec2.add(vec2.create(), vec2.fromValues(0.5, 0.5), vec2.fromValues(corner10[0], corner10[1]));
		var c01 = vec2.add(vec2.create(), vec2.fromValues(0.5, 0.5), vec2.fromValues(corner01[0], corner01[1]));
		var c11 = vec2.add(vec2.create(), vec2.fromValues(0.5, 0.5), vec2.fromValues(corner11[0], corner11[1]));

		// var ll = vec2.add(vec2.create(), vec2.fromValues(0.5, 0.5), vec2.fromValues(corner00[0], corner00[1]));
		// var ur = vec2.add(vec2.create(), vec2.fromValues(0.5, 0.5), vec2.fromValues(corner11[0], corner11[1]));
		
		// var bounds = vec2.sub(vec2.create(), ur, ll);

		// console.log(bounds);

		// var eyeCorner = [0.5 * (projCenter - lR) + 0.5, 0.5 * (eyeCenter[1]/eyeCenter[3] - lR) + 0.5];
		// console.log(eyeCorner);
		//var halfLR = 0.5 * lR;

        for (var j = 0; j < spheremap.height; j++) {
			// var v = 0.5 - ((j / spheremap.height) - 0.5);
			var v = j / spheremap.height;
			
            for (var i = 0; i < spheremap.width; i++) {
				var u = i / spheremap.height;
				
				// var uv = vec2.fromValues(u, v);
				// vec2.mul(uv, uv, bounds);
				// vec2.add(uv, uv, ll);
				
				var uv = vec2.create();
				vec2.scaleAndAdd(uv, uv, c11, u*v);
				vec2.scaleAndAdd(uv, uv, c01, (1.0-u)*v);
				vec2.scaleAndAdd(uv, uv, c10, u*(1.0-v));
				vec2.scaleAndAdd(uv, uv, c00, (1.0-u)*(1.0-v));
				
				
				// console.log(uv);
				
				// var uo = halfLR * u + eyeCorner[0];
				// var vo = halfLR * v + eyeCorner[1];

				var color = texture2D(input, uv);
				
				var destOffset = (j * spheremap.width + i) * 4;
	
				// d[destOffset]     = v * 255;
				// d[destOffset + 1] = u * 255;
				// d[destOffset + 2] = 0;
				// d[destOffset + 3] = 255;
				d[destOffset]     = color[0];
				d[destOffset + 1] = color[1];
				d[destOffset + 2] = color[2];
				d[destOffset + 3] = 255;
			}
        }

        return true;
    }
});