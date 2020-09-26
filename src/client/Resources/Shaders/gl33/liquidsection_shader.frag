﻿#version 330

out vec4 outputColor;

//in vec3 normal;

flat in int texIndex;
in vec2 texCoord;

in vec4 tint;

// binding 5
uniform sampler2DArray arrayTexture;

uniform float time;

void main()
{
	//if (mod(texCoord.x, 0.125) > 0.0625) discard;

	vec4 color = texture(arrayTexture, vec3(texCoord, texIndex + int(mod(time * 16, 16))));

	//float brightness = clamp((dot(normal, normalize(vec3(0.3, 0.8, 0.5))) + 1.7) / 2.5, 0.0, 1.0);
	//brightness = (length(normal) < 0.1) ? 1.0 : brightness;

	color *= tint;
	//color *= brightness;

	outputColor = color;
}