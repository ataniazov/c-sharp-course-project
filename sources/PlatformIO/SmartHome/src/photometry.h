#pragma once
#include <cmath>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

enum class EmitterModel
{
    SURFACE,     // Φ = E A
    ISOTROPIC,   // Φ = 4π E r^2
    LAMBERTIAN,  // Φ = π E r^2
    UNIFORM_CONE // Φ = E r^2 Ω, Ω = 2π(1 - cos(α/2))
};

// beamAngleDeg is only used for UNIFORM_CONE.
// reflectionGain > 1.0 means measured lux is boosted by reflections; we divide by it to estimate the source flux.
inline float luxToLumens(float lux,
                         float distance_m,
                         EmitterModel model = EmitterModel::LAMBERTIAN,
                         float beamAngleDeg = 180.0, // used only for UNIFORM_CONE
                         float reflectionGain = 1.0) // >1 if cavity reflections boost lux
{
    if (lux <= 0.0 || distance_m <= 0.0 || reflectionGain <= 0.0)
        return 0.0;

    const float r2 = distance_m * distance_m;
    float phi_lm = 0.0;

    switch (model)
    {
    case EmitterModel::SURFACE:
        phi_lm = lux * distance_m;
        break;
    case EmitterModel::ISOTROPIC:
        phi_lm = 4.0 * M_PI * lux * r2;
        break;
    case EmitterModel::LAMBERTIAN:
        phi_lm = M_PI * lux * r2;
        break;
    case EmitterModel::UNIFORM_CONE:
    {
        const float halfRad = (beamAngleDeg * M_PI / 180.0) * 0.5;
        const float omega = 2.0 * M_PI * (1.0 - std::cos(halfRad)); // steradians
        phi_lm = lux * r2 * omega;
        break;
    }
    }
    return phi_lm / reflectionGain; // divide out estimated boost from wall reflections (if known)
}