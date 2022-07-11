#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;

out vec3 fragPos;
out vec3 fragColor;

out mat4 model;
out mat4 view;
out mat4 proj;

uniform mat4 m;
uniform mat4 v;
uniform mat4 p;

void main()
{
    fragColor = color;

    model = m;
    view = v;
    proj = p;

    mat4 MVP = p*v*m;
    gl_Position =  MVP * vec4(position, 1.);
    fragPos = vec3(v * vec4(position, 1.0));
}