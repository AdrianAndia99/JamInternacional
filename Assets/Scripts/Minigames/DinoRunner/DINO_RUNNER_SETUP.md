# 🦖 Minijuego: Dino Runner (Escape del Dinosaurio)

## 📋 Descripción

Un juego tipo runner donde debes presionar **A** y **D** alternadamente para mover las piernas y correr. El dinosaurio te persigue y se acerca/aleja según tu rendimiento.

**Mecánica Principal:**
- Presiona **A** → **D** → **A** → **D** (alternadamente)
- ✅ **Input correcto**: El dinosaurio se aleja
- ❌ **Input incorrecto o lento**: El dinosaurio se acerca
- **Obstáculos**: Si chocas, el dinosaurio se acerca más

**Condiciones:**
- 🎉 **Victoria**: Alcanzas la distancia máxima (20m)
- 💀 **Derrota**: El dinosaurio te alcanza (distancia mínima 2m)

---

## 🎮 Cómo Funciona

### Sistema de Distancia

```
Distancia: 2m ────────────────────────────────> 20m
           💀 DERROTA              VICTORIA 🎉

- Distancia inicial: 10m
- Input correcto: +0.5m (dinosaurio se aleja)
- Input incorrecto: -1m (dinosaurio se acerca)
- Automático: -0.5m/s (el dinosaurio se acerca gradualmente)
```

### Flujo del Juego

```
Inicio
  ↓
🏃 Jugador corriendo
🦖 Dinosaurio persiguiendo (distancia: 10m)
  ↓
⌨️ Presiona A → Correcto → Dinosaurio se aleja
⌨️ Presiona D → Correcto → Dinosaurio se aleja
⌨️ Presiona A → Correcto → Dinosaurio se aleja
  ↓
❌ Presiona A cuando toca D → Error → Dinosaurio se acerca
⏰ Tardas más de 1.5s → Timeout → Dinosaurio se acerca
🚧 Chocas con obstáculo → Error → Dinosaurio se acerca
  ↓
┌─────────────────┬──────────────────┐
│ Distancia ≥ 20m │ Distancia ≤ 2m   │
│   VICTORIA ✅   │   DERROTA ❌     │
└─────────────────┴──────────────────┘
```

---

## ⚙️ Configuración en el Inspector

### 1️⃣ Referencias del Jugador

```
Player: [Transform del personaje]
Player Animator: [Animator con animaciones Run/Idle]
```

---

### 2️⃣ Referencias del Dinosaurio

```
Dinosaur: [Transform del dinosaurio]
Dinosaur Animator: [Animator con animación Run/Chase]
```

---

### 3️⃣ Configuración de Distancia

```
Initial Distance: 10
Min Distance: 2 (derrota)
Max Distance: 20 (victoria)
```

---

### 4️⃣ Configuración de Mecánica

```
Correct Input Distance: 0.5 (cuánto se aleja al presionar bien)
Wrong Input Distance: 1.0 (cuánto se acerca al fallar)
Auto Approach Speed: 0.5 (velocidad automática de acercamiento)
Max Input Delay: 1.5 (tiempo máximo entre presiones)
```

**Balanceo de Dificultad:**
```
Velocidad de acercamiento automático: 0.5m/s
Distancia ganada por input correcto: 0.5m
→ Necesitas ~1 input correcto por segundo para mantener la distancia
```

---

### 5️⃣ Configuración de Obstáculos

```
Obstacle Prefab: [Prefab del obstáculo con Collider]
Obstacle Spawn Point: [Transform adelante del jugador]
Obstacle Speed: 5
Obstacle Spawn Interval: 3 (segundos entre spawns)
```

---

### 6️⃣ UI - Indicadores

```
Next Key Text: [TextMeshPro - "Presiona: A"]
Distance Text: [TextMeshPro - "Distancia: 10m"]
Key A Indicator: [Image - Tecla A]
Key D Indicator: [Image - Tecla D]

Correct Key Color: Verde (#00FF00)
Incorrect Key Color: Rojo (#FF0000)
Neutral Key Color: Blanco (#FFFFFF)
```

---

### 7️⃣ UI - Paneles de Resultado

```
Victory Panel: [GameObject - desactivado]
Defeat Panel: [GameObject - desactivado]
```

---

### 8️⃣ Audio

```
Correct Step Audio: [AudioClipSO - sonido de paso]
Wrong Step Audio: [AudioClipSO - sonido de error]
Victory Audio: [AudioClipSO]
Defeat Audio: [AudioClipSO]
```

---

### 9️⃣ Nombres de Animaciones

```
Player Run Animation: "Run"
Player Idle Animation: "Idle"
Dinosaur Chase Animation: "Run"
```

---

## 🏗️ Setup de la Escena

### Estructura de GameObjects

```
DinoRunnerGame
├─ Player (3D Model)
│  ├─ Tag: "Player" ⚠️ IMPORTANTE
│  ├─ Animator (Run, Idle)
│  └─ Collider (Trigger) para obstáculos
│
├─ Dinosaur (3D Model)
│  ├─ Animator (Run/Chase)
│  └─ [Posición se actualiza automáticamente]
│
├─ ObstacleSpawnPoint (Empty GameObject)
│  └─ [Posición: Adelante del jugador, Z = +10]
│
├─ Canvas
│  ├─ NextKeyText (TextMeshPro)
│  │  └─ "Presiona: A"
│  ├─ DistanceText (TextMeshPro)
│  │  └─ "Distancia: 10m"
│  ├─ KeyIndicators
│  │  ├─ KeyAIndicator (Image - Tecla A)
│  │  └─ KeyDIndicator (Image - Tecla D)
│  ├─ VictoryPanel (desactivado)
│  │  ├─ VictoryText
│  │  ├─ RestartButton → DinoRunner.RestartGame()
│  │  └─ MenuButton → DinoRunner.ReturnToMenu()
│  └─ DefeatPanel (desactivado)
│     ├─ DefeatText
│     ├─ RestartButton → DinoRunner.RestartGame()
│     └─ MenuButton → DinoRunner.ReturnToMenu()
│
├─ GameManager (con script DinoRunner)
│
└─ Prefabs
   └─ Obstacle (Prefab)
      ├─ 3D Model (cubo, roca, etc.)
      ├─ Box Collider (Is Trigger: ✓)
      └─ Script ObstacleMovement se agrega automáticamente
```

---

## 🎨 Creación de la UI

### 1. Texto de Siguiente Tecla

```
NextKeyText (TextMeshPro):
- Posición: Centro-superior
- Texto inicial: "Presiona: A"
- Font Size: 72
- Color: Blanco
- Alineación: Centro
```

---

### 2. Texto de Distancia

```
DistanceText (TextMeshPro):
- Posición: Esquina superior izquierda
- Texto inicial: "Distancia: 10m\nCorrectos: 0 | Errores: 0"
- Font Size: 36
- Color: Blanco
```

---

### 3. Indicadores de Teclas

Crea dos **Images** con sprites de teclas A y D:

```
KeyAIndicator:
- Sprite: Imagen de la tecla A
- Posición: Centro-inferior izquierda
- Tamaño: 100x100
- Color inicial: Blanco

KeyDIndicator:
- Sprite: Imagen de la tecla D
- Posición: Centro-inferior derecha
- Tamaño: 100x100
- Color inicial: Blanco
```

**Comportamiento:**
- Tecla esperada → Verde
- Tecla incorrecta → Flash rojo
- Tecla neutral → Blanco

---

### 4. Paneles de Victoria/Derrota

Similar a otros minijuegos, con botones de Reintentar y Menú.

---

## 🦖 Configuración del Dinosaurio

### Posicionamiento Dinámico

El dinosaurio se posiciona automáticamente **detrás del jugador** según la distancia:

```csharp
// Posición del dinosaurio
dinosaur.position = player.position + (-player.forward * currentDistance)

// Escala (efecto de perspectiva)
scale = Lerp(1.5f, 0.5f, normalizedDistance)
```

**Resultado Visual:**
- Cerca (2m) → Dinosaurio grande (escala 1.5)
- Lejos (20m) → Dinosaurio pequeño (escala 0.5)

---

## 🚧 Creación del Obstáculo (Prefab)

### 1. Crear el Prefab

```
1. GameObject → 3D Object → Cube (o tu modelo)
2. Renombrar a "Obstacle"
3. Agregar Box Collider:
   - Is Trigger: ✓ ACTIVADO
4. Ajustar escala (ej: 1x2x1)
5. Agregar material (opcional)
6. Arrastrar a carpeta Prefabs
```

### 2. Tag del Jugador

⚠️ **IMPORTANTE**: El jugador DEBE tener el tag "Player"

```
1. Selecciona el GameObject del jugador
2. Inspector → Tag: Player
```

---

## 🎯 Mecánica de Inputs

### Sistema de Alternancia

```
Inicio: Espera "A"
  ↓
Presiona A → ✅ Correcto → Ahora espera "D"
  ↓
Presiona D → ✅ Correcto → Ahora espera "A"
  ↓
Presiona D → ❌ Error (esperaba A) → Ahora espera "D"
  ↓
Timeout (1.5s) → ❌ Error → Alterna de todos modos
```

### Casos de Error

1. **Input Incorrecto**: Presionas la tecla equivocada
2. **Timeout**: Tardas más de 1.5s en presionar
3. **Obstáculo**: Chocas con un obstáculo

Todos los errores tienen el mismo efecto: **el dinosaurio se acerca 1m**

---

## 📊 Sistema de Obstáculos

### Spawn Automático

```
Cada 3 segundos:
└─ Spawn obstáculo en ObstacleSpawnPoint
   └─ Obstáculo se mueve hacia el jugador a velocidad 5
      ├─ Si el jugador lo esquiva → Se destruye
      └─ Si choca con el jugador → Error + Se destruye
```

### Detección de Colisión

```csharp
OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        gameManager.OnObstacleHit(); // Marca como error
        Destroy(gameObject);
    }
}
```

---

## 🎮 Testing Rápido

### 1. Verificación Básica

1. **Presiona Play**
2. **Verifica en consola**: "🦖 ¡Juego iniciado!"
3. **Observa**:
   - Texto dice "Presiona: A"
   - Indicador de A en verde
   - Dinosaurio detrás del jugador

### 2. Test de Inputs

1. **Presiona A** → Debe decir "✅ ¡Correcto!"
2. **Texto cambia a**: "Presiona: D"
3. **Indicador de D** se pone verde
4. **Presiona D** → "✅ ¡Correcto!"
5. **Distancia aumenta** ligeramente

### 3. Test de Errores

1. **Presiona A dos veces seguidas** → "❌ ¡Error!"
2. **Dinosaurio se acerca**
3. **Espera 2 segundos sin presionar** → "❌ ¡Error!" (timeout)

### 4. Test de Obstáculos

1. **Espera 3 segundos** → Debe aparecer un obstáculo
2. **Obstáculo se mueve hacia el jugador**
3. **No presiones nada** → Obstáculo choca → "💥 ¡Chocaste!"
4. **Dinosaurio se acerca**

---

## ⚖️ Balanceo de Dificultad

### Configuración Actual (Media)

```
Correct Input Distance: 0.5m
Wrong Input Distance: 1.0m
Auto Approach Speed: 0.5m/s
Max Input Delay: 1.5s

Ratio: 1 error anula 2 inputs correctos
```

### Hacer más FÁCIL

```
Correct Input Distance: 1.0
Wrong Input Distance: 0.5
Auto Approach Speed: 0.3
Max Input Delay: 2.0
```

### Hacer más DIFÍCIL

```
Correct Input Distance: 0.3
Wrong Input Distance: 1.5
Auto Approach Speed: 0.8
Max Input Delay: 1.0
```

---

## 🔍 Debugging

### Mensajes en Consola

**Al iniciar:**
```
🦖 ¡Juego iniciado! Presiona A y D alternadamente para correr.
```

**Input correcto:**
```
✅ ¡Correcto! Distancia: 10.5m
```

**Input incorrecto:**
```
❌ ¡Error! El dinosaurio se acerca. Distancia: 9.0m
```

**Obstáculo:**
```
🚧 Obstáculo spawneado
💥 Obstáculo golpeó al jugador
✅ Obstáculo esquivado
```

**Victoria/Derrota:**
```
🎉 ¡VICTORIA! ¡Escapaste del dinosaurio!
💀 ¡DERROTA! ¡El dinosaurio te atrapó!
```

---

## 🐛 Troubleshooting

### El dinosaurio no se mueve
**Solución:**
- Verifica que `Dinosaur` Transform esté asignado
- Verifica que `Player` Transform esté asignado

---

### Los obstáculos no chocan
**Solución:**
- Verifica que el jugador tenga el Tag "Player"
- Verifica que el obstáculo tenga Collider con "Is Trigger" activado
- Verifica que el jugador tenga un Collider

---

### Los indicadores no cambian de color
**Solución:**
- Verifica que `Key A Indicator` y `Key D Indicator` estén asignados
- Verifica que sean componentes Image (no RawImage)

---

### El input no responde
**Solución:**
- Verifica que el juego esté activo (gameActive = true)
- Verifica en consola si detecta las teclas presionadas
- Asegúrate de que no haya otro script capturando los inputs

---

## 💡 Tips para el Jugador

1. **Mantén el ritmo** - No presiones demasiado rápido ni demasiado lento
2. **Lee el indicador** - La tecla verde es la correcta
3. **No presiones dos veces la misma tecla**
4. **Anticipa los obstáculos** - Si ves uno venir, mantén el ritmo para esquivarlo
5. **No te detengas** - El dinosaurio se acerca automáticamente

---

## 📐 Matemática del Sistema

```
Distancia después de N segundos sin inputs:

distancia = 10m - (0.5m/s × N segundos)

Ejemplos:
- 5 segundos sin inputs: 10m - 2.5m = 7.5m
- 10 segundos sin inputs: 10m - 5m = 5m
- 16 segundos sin inputs: 10m - 8m = 2m (DERROTA)

Para mantener la distancia:
1 input correcto (+0.5m) cada 1 segundo
```

---

¡A correr del dinosaurio! 🏃‍♂️🦖💨
