#version 330 core

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout(location = 0) in vec3 iPosition;
layout(location = 1) in vec2 iTexCoord;
layout(location = 2) in vec3 iBlockSpec;
layout(location = 3) in float texLayer;

out vec2 oTexCoord;
out float otexLayer;
//out vec4 oColor;
out vec3 oFragPos;

void main(void)
{
    gl_Position = projection * view * model * vec4(iPosition+iBlockSpec, 1.0);
	oTexCoord = iTexCoord;
	otexLayer = texLayer;
	oFragPos = vec3(model * vec4(iPosition+iBlockSpec, 1.0));

	//vec4 pos = model * vec4(iPosition + iBlockSpec, 1.0);
	//oColor = vec4(min(pos.y*.025, 0.5) + min(0.5, max(0, pos.y-128)*.004), 
	//			0.4 + min(0.1, max(0, pos.y-20)*0.001) + min(0.5, max(0, pos.y-128)*0.004), 
	//			max(0, 1-pos.y*0.05) + min(1, max(0,pos.y-20)*0.004275), 
	//			1.0);

}