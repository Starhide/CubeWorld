#version 330

uniform sampler2DArray texture0;
uniform vec3 sunLightPos;
uniform vec3 normal;
uniform vec3 viewPos;

in vec2 oTexCoord;
in float otexLayer;
//in vec4 oColor;
in vec3 oFragPos;

out vec4 outputColor;


void main()
{
	float ambientstr = 0.25;
	vec3 ambient = ambientstr * vec3(1.0, 1.0, 1.0);

	vec3 norm = normalize(normal);
	vec3 lightDir = normalize(sunLightPos);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = diff * vec3(1.0, 1.0, 1.0);

	vec3 light = ambient + diffuse;
    outputColor =  vec4(light, 1.0) * texture(texture0, vec3(oTexCoord, otexLayer));
}