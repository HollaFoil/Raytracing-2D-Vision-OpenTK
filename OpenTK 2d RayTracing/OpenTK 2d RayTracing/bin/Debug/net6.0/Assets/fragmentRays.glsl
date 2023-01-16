#version 330
in vec2 vPos;
out vec4 outputColor;

uniform vec2 center;
uniform vec4 color;

void main()
{
    float dist = 1-distance(vPos, center);
    vec4 state = color*dist;
    outputColor = vec4(state.r, state.g, state.b, state.a*dist);
}