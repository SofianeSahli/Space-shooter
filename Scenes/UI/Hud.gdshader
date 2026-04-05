shader_type canvas_item;

uniform vec4 radar_color : source_color = vec4(0.0, 1.0, 0.0, 1.0);
uniform float speed : hint_range(0.1, 5.0) = 1.0;
uniform float line_thickness : hint_range(0.001, 1) = 0.01;
uniform float radar_radius = 1.0;

void fragment() {
    // 1. Center UVs to (0,0) and handle aspect ratio
    vec2 uv = UV * 2.0 - 1.0;
    
    // 2. Calculate distance and angle
    float dist = length(uv);
  // =========================
    // 🟢 CIRCLE MASK
    // =========================
    float mask = step(dist, radar_radius);

    // =========================
    // 🧭 ANGLE (0 → 1)
    // =========================
    float angle = atan(uv.y, uv.x);    
    // 3. Rotating sweep (time-based)
    float rot = TIME * speed;
    float sweep = sin(angle + rot);
    
    // 4. Create sweep line
    float radar_line = smoothstep(1.0 - line_thickness, 1.0, sweep);
    
    // 5. Add concentric rings
    float rings = cos(dist * 20.0 ) * 0.5 + 0.5;
    rings = pow(rings, 20.0); //Sharpen rings
    
    // 6. Combine and Apply Color
    float intensity = radar_line * 0.5 + rings * 0.2;
    intensity += (1.0 - dist) * 0.1; // Slight inner glow
    
    // Simple Circular Mask
    
    COLOR = vec4(radar_color.rgb * intensity * mask, mask);
}
