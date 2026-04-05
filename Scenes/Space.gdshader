shader_type canvas_item;

uniform float star_density = 1.0; // 🔥 LOW = fewer stars
uniform float min_star_size = 0.02;
uniform float max_star_size = 0.08;

uniform float brightness = 5.2;

// barely moving (almost static)
uniform vec2 drift = vec2(0.001, 0.001);
uniform float drift_speed = 0.02;

float rand(vec2 co)
{
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment()
{
    vec2 uv = UV;

    // very slow movement
    uv += TIME * drift * drift_speed;

    // grid
    vec2 grid_uv = uv * star_density;

    vec2 cell = floor(grid_uv);
    vec2 f = fract(grid_uv);

    // random values per star
    float r = rand(cell);
    float r2 = rand(cell + 1.0);
    float r3 = rand(cell + 2.0);

    // star size
    float size = mix(min_star_size, max_star_size, r);

    // distance from center of cell
    float dist = length(f - 0.5);

    // only keep a few stars (THIS reduces density further)
    float star_mask = step(0.85, r3);

    // star shape
    float star = smoothstep(size, 0.0, dist);

    // brightness variation
    float intensity = star * star_mask * brightness;

    // subtle flicker
    float flicker = sin(TIME * (r2 * 2.0 + 1.0)) * 0.5 + 0.5;
    intensity *= flicker;

    // slight color variation (white / blue / warm)
    vec3 star_color = mix(
        vec3(1.0, 1.0, 1.0),
        vec3(0.8, 0.9, 1.0),
        r
    );

    star_color = mix(
        star_color,
        vec3(1.0, 0.95, 0.8),
        r2 * 0.3
    );

    vec3 color = star_color * intensity;

    // dark space background
    color += vec3(0.01, 0.01, 0.03);

    COLOR = vec4(color, 1.0);
}