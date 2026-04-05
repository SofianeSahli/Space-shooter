shader_type canvas_item;

uniform vec4 color : source_color = vec4(1.0, 0.2, 0.2, 1.0);
uniform float glow = 2.0;
uniform float pulse_speed = 6.0;
uniform float thickness = 18.3;

void fragment()
{
    vec2 uv = UV;
	uv.x = 20.0;
    float dist = abs(uv.y - 0.5);
    float core = smoothstep(thickness, 0.0, dist);
    float glow_part = smoothstep(thickness * 2.5, 0.0, dist);
    float pulse = sin((uv.x * 10.0) - TIME * pulse_speed) * 0.5 + 0.5;
    float intensity = core + glow_part * glow;
    intensity *= mix(0.8, 1.2, pulse);
    vec3 final_color = color.rgb * intensity;
    COLOR = vec4(final_color, intensity);
}