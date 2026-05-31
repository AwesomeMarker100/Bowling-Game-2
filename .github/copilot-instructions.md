# Copilot Instructions

## Project Guidelines
- User's physics engine (ValkyrieRigidbody2) uses a shared ValkyrieCollision info structure where both objects process the same collision data, so they automatically receive opposite direction impulses through the collision system rather than explicitly applying opposite forces to both objects.