#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texScene;
uniform int width;
uniform int height;

void main()
{
    vec4 color = texture(texScene, TexCoords);
    FragColor = color;//vec4(TexCoords.x, TexCoords.y, 0., 1.);
}