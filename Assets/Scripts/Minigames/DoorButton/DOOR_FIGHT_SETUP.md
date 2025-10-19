# 🚪 Minijuego: Door Fight (Jalar la Puerta)

## 📋 Descripción

Un minijuego estilo "jalar la soga" donde el jugador debe mantener una puerta abierta durante 10 segundos mientras la IA intenta cerrarla.

**Mecánica:**
- La puerta rota desde 0° hasta -90°
- La IA empuja gradualmente (6% por segundo)
- El jugador presiona un botón para empujar de vuelta (reduce de 1% a 5% aleatorio)
- Si la puerta llega a -90° → **DERROTA**
- Si pasaron 10 segundos sin llegar a -90° → **VICTORIA**

---

## 🎮 Cómo Funciona

### Sistema de Porcentajes

```
IA: 0% ────────────────────────────────> 100%
Puerta: 0° ──────────────────────────> -90°

- 0% de IA = Puerta en 0° (abierta)
- 50% de IA = Puerta en -45° (mitad)
- 100% de IA = Puerta en -90° (cerrada = DERROTA)
```

### Flujo del Juego

```
Inicio del Juego
    ↓
⏱️ Timer: 10 segundos
🤖 IA: +6% por segundo (automático)
    ↓
🎯 Jugador: Presiona botón
    ├─ Reduce IA: -1% a -5% (aleatorio)
    └─ Aumenta su %: +1% a +5%
    ↓
🚪 Puerta rota según % de IA
    ├─ 0% → Puerta en 0°
    └─ 100% → Puerta en -90°
    ↓
┌────────────────┬─────────────────┐
│ Tiempo agotado │ Puerta a -90°   │
│ (10 segundos)  │ (Antes de 10s)  │
│   VICTORIA ✅  │   DERROTA ❌    │
└────────────────┴─────────────────┘
```

---

## ⚙️ Configuración en el Inspector

### 1️⃣ Referencias de la Puerta

```
Door: [Transform de la puerta]
Initial Angle: 0
Defeat Angle: -90
```

- **Door**: El Transform que va a rotar (debe rotar en el eje Y local)
- **Initial Angle**: Ángulo inicial (0° = puerta abierta)
- **Defeat Angle**: Ángulo de derrota (-90° = puerta completamente cerrada)

---

### 2️⃣ Configuración del Juego

```
Game Duration: 10
AI Increment Speed: 6
Min Player Decrement: 1
Max Player Decrement: 5
```

- **Game Duration**: Duración del juego en segundos (default: 10s)
- **AI Increment Speed**: Velocidad de incremento de la IA por segundo (default: 6%/s)
- **Min/Max Player Decrement**: Rango de reducción al presionar botón (1-5%)

**Cálculo de Dificultad:**
```
IA incrementa 6% por segundo × 10 segundos = 60% total
Jugador puede reducir de 1% a 5% por click

Para ganar, necesitas hacer muchos clicks rápidos!
```

---

### 3️⃣ UI - Textos de Porcentaje

```
Player Percentage Text: [TextMeshPro]
AI Percentage Text: [TextMeshPro]
Timer Text: [TextMeshPro] (opcional)
```

**Formato esperado:**
```
Jugador: 45%
IA: 55%
Tiempo: 7.3s
```

---

### 4️⃣ UI - Botón de Acción

```
Action Button: [Button]
Button Text: [TextMeshPro] (opcional)
```

- **Action Button**: El botón que el jugador debe presionar repetidamente
- **Button Text**: Texto del botón (muestra "¡JALA!" normalmente, y "-X%" al presionar)

---

### 5️⃣ UI - Paneles de Resultado

```
Victory Panel: [GameObject]
Defeat Panel: [GameObject]
```

Ambos paneles deben:
- Estar **desactivados** por default
- Contener texto, botones (Reintentar, Menú), etc.

---

### 6️⃣ Audio (Opcional)

```
Victory Audio: [AudioClipSO]
Defeat Audio: [AudioClipSO]
```

---

## 🏗️ Setup de la Escena

### Estructura de GameObjects

```
DoorFightGame
├─ Door (Transform con modelo 3D)
│  └─ [El modelo visual de la puerta]
├─ Canvas
│  ├─ PlayerPercentageText (TextMeshPro)
│  ├─ AIPercentageText (TextMeshPro)
│  ├─ TimerText (TextMeshPro)
│  ├─ ActionButton (Button)
│  │  └─ ButtonText (TextMeshPro: "¡JALA!")
│  ├─ VictoryPanel (GameObject - desactivado)
│  │  ├─ VictoryText ("¡VICTORIA!")
│  │  ├─ RestartButton → DoorFight.RestartGame()
│  │  └─ MenuButton → DoorFight.ReturnToMenu()
│  └─ DefeatPanel (GameObject - desactivado)
│     ├─ DefeatText ("¡DERROTA!")
│     ├─ RestartButton → DoorFight.RestartGame()
│     └─ MenuButton → DoorFight.ReturnToMenu()
└─ GameManager (con script DoorFight)
```

---

## 🎨 Creación de la UI

### 1. Textos de Porcentaje

**Crear dos TextMeshPro:**

```
GameObject → UI → Text - TextMeshPro

PlayerPercentageText:
- Posición: Arriba a la izquierda
- Texto inicial: "Jugador: 0%"
- Color: Verde (#00FF00)
- Font Size: 48

AIPercentageText:
- Posición: Arriba a la derecha
- Texto inicial: "IA: 0%"
- Color: Rojo (#FF0000)
- Font Size: 48
```

---

### 2. Temporizador (Opcional)

```
TimerText:
- Posición: Centro superior
- Texto inicial: "Tiempo: 10.0s"
- Color: Blanco
- Font Size: 36
```

---

### 3. Botón de Acción

```
GameObject → UI → Button - TextMeshPro

ActionButton:
- Posición: Centro de la pantalla (fácil acceso)
- Tamaño: 200x100
- Texto: "¡JALA!"
- Color del botón: Amarillo/Naranja
- Font Size del texto: 48
```

**Importante:** Este botón debe ser grande y fácil de presionar rápidamente.

---

### 4. Panel de Victoria

```
GameObject → UI → Panel

VictoryPanel:
- Tamaño: Fullscreen
- Color de fondo: Semi-transparente negro (50%)
- Desactivado por default

Dentro del VictoryPanel:
└─ VictoryText (TextMeshPro)
   - Texto: "¡VICTORIA!\n¡Mantuviste la puerta abierta!"
   - Color: Dorado (#FFD700)
   - Font Size: 64
   - Alineación: Centro

└─ RestartButton (Button)
   - Texto: "Reintentar"
   - OnClick: DoorFight.RestartGame()

└─ MenuButton (Button)
   - Texto: "Menú Principal"
   - OnClick: DoorFight.ReturnToMenu()
```

---

### 5. Panel de Derrota

```
GameObject → UI → Panel

DefeatPanel:
- Tamaño: Fullscreen
- Color de fondo: Semi-transparente rojo oscuro (50%)
- Desactivado por default

Dentro del DefeatPanel:
└─ DefeatText (TextMeshPro)
   - Texto: "¡DERROTA!\n¡La puerta se cerró completamente!"
   - Color: Rojo (#FF0000)
   - Font Size: 64
   - Alineación: Centro

└─ RestartButton (Button)
   - Texto: "Reintentar"
   - OnClick: DoorFight.RestartGame()

└─ MenuButton (Button)
   - Texto: "Menú Principal"
   - OnClick: DoorFight.ReturnToMenu()
```

---

## 🚪 Configuración de la Puerta

### Pivot Point de la Puerta

**IMPORTANTE:** La puerta debe rotar desde uno de sus bordes, no desde el centro.

```
Door (Empty GameObject)
└─ DoorModel (Modelo 3D)
```

1. Crea un **Empty GameObject** llamado "Door"
2. Coloca el pivot en el **borde izquierdo** de donde quieres que rote
3. Arrastra el modelo 3D de la puerta como **hijo** del Empty GameObject
4. Ajusta la posición del modelo para que el borde coincida con el pivot del padre

**Rotación:**
- La puerta rota en el eje **Y local**
- 0° = Puerta abierta (paralela a la pared)
- -90° = Puerta cerrada (perpendicular a la pared)

---

## 🎮 Testing Rápido

### Verificación de Configuración

1. **Presiona Play**
2. **Verifica en consola**: "🎮 ¡Juego iniciado! Mantén la puerta abierta por 10 segundos."
3. **Observa**:
   - IA% debe incrementar gradualmente (6% por segundo)
   - Puerta debe rotar hacia -90°
4. **Presiona el botón varias veces**:
   - IA% debe reducirse de 1% a 5%
   - Jugador% debe incrementarse
   - Puerta debe rotar de vuelta hacia 0°
5. **Espera 10 segundos SIN dejar que llegue a -90°**:
   - Debe aparecer el panel de Victoria
6. **Reinicia y NO presiones el botón**:
   - En ~16 segundos la puerta llegará a -90°
   - Debe aparecer el panel de Derrota

---

## 📊 Debugging

### Mensajes en Consola

**Al iniciar:**
```
🎮 ¡Juego iniciado! Mantén la puerta abierta por 10 segundos.
```

**Al presionar botón:**
```
🎯 Jugador presionó el botón! Redujo 3% a la IA
```

**Al ganar:**
```
🎉 ¡VICTORIA! Mantuviste la puerta abierta.
```

**Al perder:**
```
💀 ¡DERROTA! La IA cerró la puerta completamente.
```

---

## ⚖️ Balanceo de Dificultad

### Dificultad Actual (Default)

```
IA: +6% por segundo
Jugador: -1% a -5% por click
Duración: 10 segundos

Incremento total de IA sin interferencia: 60%
Clicks necesarios para contrarrestar: ~12-60 clicks en 10s
```

### Ajustar Dificultad

**Hacer más FÁCIL:**
```
AI Increment Speed: 4
Min Player Decrement: 3
Max Player Decrement: 7
Game Duration: 12
```

**Hacer más DIFÍCIL:**
```
AI Increment Speed: 8
Min Player Decrement: 1
Max Player Decrement: 3
Game Duration: 8
```

---

## 🎯 Tips para el Jugador

1. **Presiona el botón RÁPIDAMENTE** - Cada click cuenta
2. **No esperes** a que la IA llegue a 100%
3. **Mantén el ritmo** durante los 10 segundos
4. **Observa el temporizador** - Solo necesitas sobrevivir 10 segundos

---

## 🔧 Troubleshooting

### La puerta no rota
**Solución:**
- Verifica que el `Door` Transform esté asignado
- Verifica que el pivot esté en el borde correcto
- Verifica que rote en el eje Y local

---

### El botón no responde
**Solución:**
- Verifica que `Action Button` esté asignado
- Verifica que el EventSystem esté en la escena
- Verifica que el botón esté `Interactable`

---

### Los porcentajes no se actualizan
**Solución:**
- Verifica que `Player Percentage Text` y `AI Percentage Text` estén asignados
- Verifica que sean TextMeshPro (no Text normal)

---

### El juego termina inmediatamente
**Solución:**
- Verifica que `Initial Angle` sea 0 y `Defeat Angle` sea -90
- Verifica que la puerta no esté ya en -90° al inicio

---

## 📐 Matemática del Sistema

```csharp
// Porcentaje de IA → Ángulo de puerta
currentAngle = Lerp(0°, -90°, aiPercentage / 100f)

Ejemplos:
- 0% de IA → Puerta en 0° (totalmente abierta)
- 25% de IA → Puerta en -22.5°
- 50% de IA → Puerta en -45°
- 75% de IA → Puerta en -67.5°
- 100% de IA → Puerta en -90° (DERROTA)
```

---

¡Diversión garantizada! 🎮🚪✨
