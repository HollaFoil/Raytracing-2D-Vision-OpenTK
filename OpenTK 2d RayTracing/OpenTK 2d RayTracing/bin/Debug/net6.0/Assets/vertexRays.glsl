#version 330 core
layout (location = 0) in vec2 aPos;
out vec2 vPos;

void main()
{
    vPos = aPos;
    gl_Position = vec4(aPos.x, aPos.y, 0, 1.0);
}