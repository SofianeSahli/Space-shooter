shader_type canvas_item;

uniform float size = 0.8;
uniform float stretch = 1.5;

void fragment()
{
    vec2 uv = UV;

    // 🔥 stretch vertically
    uv.y = (uv.y - 0.5) * stretch + 0.5;

    float dist = distance(uv, vec2(0.5));

float alpha = smoothstep(0.6, 0.0, dist);

    vec3 color = mix(
        vec3(1.0, 0.5, 0.1),
        vec3(0.2, 0.2, 0.2),
        dist
    );
	uv.x += sin(TIME * 10.0 + uv.y * 5.0) * 0.01;
alpha = pow(alpha, 1.5); // smooth edges more
    COLOR = vec4(color, alpha * 0.6);
}