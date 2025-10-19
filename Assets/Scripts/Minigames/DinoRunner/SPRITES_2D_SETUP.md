# 🦖 Dino Runner - Configuración para Sprites 2D en Entorno 3D

## 📋 Diferencias con Modelos 3D

Cuando usas **sprites 2D en un entorno 3D**, necesitas considerar:

1. **Billboard Effect**: Los sprites deben mirar siempre a la cámara
2. **Movimiento en 2D**: Solo usamos ejes X e Y (no Z en profundidad)
3. **Colliders 2D vs 3D**: Puedes usar BoxCollider2D o BoxCollider según tu setup
4. **Escala**: Mantener la proporción del sprite (no deformar)

---

## 🎨 Setup de Sprites 2D

### Opción 1: Vista Lateral (Plano XY) - RECOMENDADO

Este es el setup típico de juegos 2D tipo "runner":

```
Camera:
- Position: (0, 0, -10)
- Rotation: (0, 0, 0)
- Projection: Orthographic
- Size: 5

Player:
- Position: (0, 0, 0)
- Sprite Renderer
- Box Collider 2D o Box Collider
- Tag: "Player"

Dinosaur:
- Position: (-10, 0, 0) ← 10 unidades a la IZQUIERDA
- Sprite Renderer
- Se actualiza automáticamente
```

**Ejes:**
- **X**: Horizontal (izquierda ← → derecha)
- **Y**: Vertical (abajo ↓ ↑ arriba)
- **Z**: Profundidad (hacia/desde cámara) - NO SE USA para movimiento

---

### Opción 2: Vista Isométrica (Plano XZ)

Si quieres una vista más "3D" pero con sprites:

```
Camera:
- Position: (0, 10, -10)
- Rotation: (45, 0, 0)
- Projection: Perspective o Orthographic

Player:
- Position: (0, 0, 0)
- Los sprites "paran" en el suelo

Dinosaur:
- Position: (-10, 0, 0)
```

---

## ⚙️ Configuración del Código

### Sistema de Posicionamiento

En `DinoRunner.cs`, el método `UpdateDinosaurPosition()` ahora tiene dos opciones:

```csharp
// OPCIÓN 1: Vista lateral (XY) - DEFAULT
Vector3 offset = new Vector3(-currentDistance, 0f, 0f);

// OPCIÓN 2: Vista isométrica (XZ) - Descomentar si usas esta
// Vector3 offset = new Vector3(-currentDistance, 0f, 0f);
```

### Billboard Effect

```csharp
// Hace que el sprite siempre mire a la cámara
if (Camera.main != null)
{
    dinosaur.rotation = Camera.main.transform.rotation;
}
```

Esto evita que los sprites se vean "de lado" en un entorno 3D.

---

## 🚧 Setup de Obstáculos para Sprites 2D

### 1. Crear el Prefab del Obstáculo

```
GameObject: Obstacle
├─ Sprite Renderer (tu sprite de obstáculo)
├─ Box Collider 2D o Box Collider
│  └─ Is Trigger: ✓ ACTIVADO
└─ Tag: (no necesita tag)
```

### 2. Configuración del Collider

**Opción A: Box Collider 2D** (para físicas 2D puras)
```
Box Collider 2D:
- Size: (1, 1) - Ajusta según tu sprite
- Is Trigger: ✓
```

**Opción B: Box Collider** (para entorno 3D mixto)
```
Box Collider:
- Size: (1, 1, 0.5)
- Is Trigger: ✓
```

### 3. Movimiento del Obstáculo

En `ObstacleMovement.cs`, el obstáculo se mueve así:

```csharp
// Mueve en el eje X (hacia la derecha, hacia el jugador)
transform.position += Vector3.right * speed * Time.deltaTime;
```

**Posición de Spawn:**
```
ObstacleSpawnPoint:
- Position: (-5, 0, 0) ← A la IZQUIERDA del jugador
```

Los obstáculos aparecen a la izquierda y se mueven hacia la derecha (hacia el jugador).

---

## 🎮 Layout de la Escena (Vista Lateral)

```
          Obstáculo
               ↓
    [🚧] ←──────────── (spawned aquí)
      ↓
      🚧 ←── velocidad
       ↓
        🚧
         ↓
    🦖 ←── dinosaurio (se acerca/aleja)
     ↓
    🏃 ←── jugador (posición fija)

IZQUIERDA ←───────────→ DERECHA
    -X                    +X
```

---

## 📐 Configuración de la Cámara

### Para Vista Lateral (Orthographic)

```
Main Camera:
├─ Position: (0, 0, -10)
├─ Rotation: (0, 0, 0)
├─ Projection: Orthographic ← IMPORTANTE
├─ Size: 5 (ajusta según necesites)
└─ Clear Flags: Solid Color o Skybox
```

**¿Por qué Orthographic?**
- No hay perspectiva (objetos lejanos no se ven más pequeños)
- Perfecto para juegos 2D
- Los sprites mantienen su tamaño

---

### Para Vista Isométrica (Perspective)

```
Main Camera:
├─ Position: (0, 8, -8)
├─ Rotation: (45, 0, 0)
├─ Projection: Perspective
├─ Field of View: 60
└─ Clear Flags: Solid Color o Skybox
```

---

## 🔧 Sprite Renderer Setup

### Player y Dinosaur

```
Sprite Renderer:
├─ Sprite: [Tu sprite]
├─ Color: White (1, 1, 1, 1)
├─ Flip X: ☐ (depende de tu sprite)
├─ Flip Y: ☐
├─ Material: Sprites-Default
└─ Sorting Layer: Default
    Order in Layer: 0 (player), 1 (dinosaur)
```

**Orden de Capas:**
- Fondo: -10
- Dinosaurio: 0
- Player: 1
- Obstáculos: 2
- UI: 100

---

## 🎨 Animator Setup para Sprites 2D

### Player Animator

```
Animator:
├─ Controller: PlayerAnimator
│  ├─ Estado: Idle (sprite estático)
│  ├─ Estado: Run (animación de correr)
│  └─ Transiciones automáticas desde el código
└─ Apply Root Motion: ☐ DESACTIVADO
```

### Animación de Correr

Crea una **animación de sprites** con frames:

```
Run Animation:
├─ Frame 1: Sprite pierna izquierda adelante
├─ Frame 2: Sprite pierna derecha adelante
├─ Frame 3: Sprite pierna izquierda adelante
└─ Loop: ✓
   Speed: 12 fps
```

---

## 🔍 Colliders: 2D vs 3D

### ¿Cuándo usar cada uno?

**Box Collider 2D:**
```
Pros:
✅ Más eficiente para juegos 2D puros
✅ Usa el sistema de físicas 2D de Unity
✅ Solo detecta otros Collider2D

Contras:
❌ No funciona con Collider 3D
❌ Requiere OnTriggerEnter2D()
```

**Box Collider (3D):**
```
Pros:
✅ Funciona en entornos 3D mixtos
✅ Compatible con Collider 3D
✅ Más flexible

Contras:
❌ Menos eficiente que 2D
❌ Requiere OnTriggerEnter() (3D)
```

**Recomendación:** Usa **Box Collider 2D** si todo tu juego es 2D, incluso en un entorno 3D.

---

## 📝 Checklist de Configuración

### ✅ Setup Básico

- [ ] Cámara en modo Orthographic
- [ ] Player con Sprite Renderer
- [ ] Dinosaur con Sprite Renderer
- [ ] Player con tag "Player"
- [ ] ObstacleSpawnPoint a la izquierda del jugador

### ✅ Colliders

- [ ] Player: Box Collider 2D (o Box Collider)
- [ ] Obstáculo Prefab: Box Collider 2D con Is Trigger ✓
- [ ] Tamaños de colliders ajustados a los sprites

### ✅ Animaciones

- [ ] Player: Animator con estados Run/Idle
- [ ] Dinosaur: Animator con estado Run/Chase
- [ ] Animaciones funcionan correctamente

### ✅ Script

- [ ] DinoRunner.cs asignado a GameManager
- [ ] Referencias asignadas (Player, Dinosaur, etc.)
- [ ] ObstacleSpawnPoint configurado
- [ ] Obstacle Prefab asignado

---

## 🎯 Ajustes de Posición

### Posiciones Iniciales Recomendadas

```
Player:
- Position: (0, 0, 0)

Dinosaur (calculado automáticamente):
- Position: (-10, 0, 0) ← 10 unidades a la izquierda

ObstacleSpawnPoint:
- Position: (-5, 0, 0) ← Entre el jugador y el dinosaurio
```

### Escala de Sprites

Si tus sprites se ven muy grandes o pequeños:

```
Player/Dinosaur:
- Scale: (0.1, 0.1, 1) ← Ajusta según tu sprite
- Pixels Per Unit: 100 (en import settings del sprite)
```

---

## 🚀 Testing

### 1. Verificar Billboard

1. **Presiona Play**
2. **Mueve la Scene View** con el mouse
3. **Verifica**: Los sprites deben SIEMPRE mirar a la cámara
4. No deben verse "de lado" nunca

### 2. Verificar Movimiento

1. **Presiona Play**
2. **Observa**: El dinosaurio está a la izquierda del jugador
3. **Presiona A/D**: El dinosaurio se acerca/aleja HORIZONTALMENTE
4. No debe moverse en Z (profundidad)

### 3. Verificar Obstáculos

1. **Espera 3 segundos**
2. **Obstáculo aparece** a la izquierda
3. **Se mueve** hacia la derecha (hacia el jugador)
4. **Choca o pasa** al jugador

---

## 🐛 Problemas Comunes

### Los sprites se ven de lado

**Problema:** El billboard no funciona  
**Solución:**
```csharp
// Verifica que esto esté en UpdateDinosaurPosition()
if (Camera.main != null)
{
    dinosaur.rotation = Camera.main.transform.rotation;
}
```

---

### Los obstáculos no chocan

**Problema:** Colliders 2D y 3D mezclados  
**Solución:**
- Player: Box Collider 2D → Obstáculo: Box Collider 2D
- Player: Box Collider → Obstáculo: Box Collider
- NO mezcles tipos

---

### El dinosaurio no se mueve en el eje correcto

**Problema:** Offset mal configurado  
**Solución:**
```csharp
// Para vista lateral (XY), debe ser:
Vector3 offset = new Vector3(-currentDistance, 0f, 0f);

// Si no funciona, prueba:
// Vector3 offset = new Vector3(0f, 0f, -currentDistance);
```

---

### Los sprites se deforman al escalar

**Problema:** Escala incorrecta  
**Solución:**
```csharp
// Mantén Z = 1 para sprites 2D
dinosaur.localScale = new Vector3(scale, scale, 1f);
```

---

## 📊 Comparación: 2D Puro vs 2D en 3D

### Setup 2D Puro

```
✅ Usa Collider2D
✅ OnTriggerEnter2D()
✅ Camera Orthographic
✅ Todo en el plano XY
✅ Physics 2D
```

### Setup 2D en 3D (Tu caso)

```
✅ Puede usar Collider2D o Collider
✅ OnTriggerEnter() y OnTriggerEnter2D()
✅ Camera Orthographic o Perspective
✅ Billboard para sprites
✅ Puede mezclar 2D y 3D
```

---

¡Listo para sprites 2D en tu runner! 🏃‍♂️🦖✨
