# 🦖 Sistema de Enemigos y Victoria - Godzilla

## 📋 Scripts Creados

1. **GodzillaEnemy.cs** - Controla el movimiento y destrucción del enemigo
2. **GodzillaGameManager.cs** - Gestiona victoria, derrota y UI
3. **GodzillaController.cs** - Actualizado para detectar enemigos

---

## 🔧 Configuración Paso a Paso

### Paso 1: Crear el Enemigo

1. **Crea o usa tu modelo de enemigo** en la escena
2. **Agrega un Collider**:
   - Si no tiene, agrega: Add Component → Collider (Box, Sphere, Capsule)
   - ✅ Asegúrate que **Is Trigger** esté **DESACTIVADO**
3. **Agrega el script [`GodzillaEnemy`](Assets/Scripts/Minigames/GodzillaEnemy.cs )**

---

### Paso 2: Configurar Puntos de Movimiento

1. **Crea dos GameObjects vacíos**:
   - Click derecho en Hierarchy → Create Empty
   - Nombra uno **"PointA"**
   - Nombra el otro **"PointB"**

2. **Posiciona los puntos**:
   - Coloca PointA en un extremo del recorrido
   - Coloca PointB en el otro extremo
   - El enemigo se moverá entre estos dos puntos

3. **En el Inspector del Enemigo**:
   ```
   GodzillaEnemy (Script)
   ├─ Configuración de Movimiento
   │  ├─ Point A: [Arrastra PointA aquí] ⚠️
   │  ├─ Point B: [Arrastra PointB aquí] ⚠️
   │  ├─ Move Speed: 2
   │  └─ Movement Type: PingPong (o Loop)
   ```

---

### Paso 3: Configurar el GameManager

1. **Crea un GameObject vacío** en la escena:
   - Click derecho en Hierarchy → Create Empty
   - Nombre: **"GameManager"**

2. **Agrega el script [`GodzillaGameManager`](Assets/Scripts/Minigames/GodzillaGameManager.cs )**

3. **Crear Panel de Victoria**:
   - Click derecho en Hierarchy → UI → Panel
   - Nombre: **"VictoryPanel"**
   - Configúralo visualmente:
     - Agrega texto: Click derecho en VictoryPanel → UI → Text
     - Texto: "¡VICTORIA!"
     - Tamaño de fuente grande, color amarillo/dorado
   - **Desactívalo**: Desmarca el checkbox en el Inspector

4. **En el Inspector del GameManager**:
   ```
   GodzillaGameManager (Script)
   ├─ Referencias UI
   │  ├─ Victory Panel: [Arrastra VictoryPanel] ⚠️
   │  └─ Victory Text: [Arrastra el Text hijo]
   │
   ├─ Audio
   │  ├─ Victory Roar Scene Name: "GodzillaVictory"
   │  └─ Victory Roar Audio: [Opcional: AudioClipSO directo]
   │
   └─ Referencias
      └─ Godzilla Controller: [Arrastra el GameObject Godzilla]
   ```

---

### Paso 4: Configurar Audio de Victoria en SoundsController

1. **Encuentra el GameObject SoundsController** en tu escena
2. **Agrega una nueva entrada** en Scene Audio Entries:
   ```
   Element [nuevo]:
   ├─ Scene Name: "GodzillaVictory"
   ├─ Audio Clip: [Tu AudioClipSO del rugido de victoria]
   ├─ Play As Loop: ☐ (desactivado)
   └─ Trigger On Scene Loaded: ☐ (desactivado)
   ```

---

### Paso 5: Configurar el LineRenderer para Colisiones

Para que el láser detecte enemigos correctamente:

1. **Selecciona el GameObject del láser** (LaserBeam)
2. **Agrega un Collider en forma de tubo** (opcional pero recomendado):
   - Add Component → Capsule Collider
   - **Is Trigger**: ☑️ ACTIVADO
   - Rotar para que sea horizontal
   - Ajustar tamaño para que cubra el ancho del láser

3. **O simplemente usa Raycast** (el sistema ya lo hace automáticamente)

---

## 🎮 Flujo del Juego

```
┌─────────────────────────────────────────────────────────┐
│  1. INICIO                                              │
│  ├─ Godzilla rota automáticamente                      │
│  └─ Enemigo se mueve entre PointA y PointB             │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  2. JUGADOR PRESIONA ESPACIO                            │
│  ├─ Godzilla se detiene                                 │
│  ├─ Audio "Godzilla" (25s) comienza                     │
│  └─ Animaciones: Idle → Init → Shoot_Previous           │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  3. SEGUNDO 16: ¡LÁSER DISPARA!                         │
│  ├─ Raycast detecta todos los objetos en el camino     │
│  ├─ Si encuentra enemigo → enemy.DestroyEnemy()        │
│  ├─ GameManager.OnEnemyDestroyed() se llama            │
│  └─ Flag: enemyDestroyedDuringAttack = true            │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  4. SEGUNDO 25: FIN DE SECUENCIA                        │
│  ├─ Láser se apaga                                      │
│  ├─ Audio termina                                       │
│  └─ GodzillaController.FinishAttackSequence()          │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  5. VERIFICACIÓN DE VICTORIA                            │
│  ├─ GameManager.OnAttackSequenceComplete()             │
│  ├─ ¿Enemigo destruido? SI → Continuar                 │
│  ├─ ¿Todos los enemigos muertos? SI → ¡VICTORIA!       │
│  └─ NO → Volver a rotar (puede intentar de nuevo)      │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  6. ¡VICTORIA!                                          │
│  ├─ Audio "GodzillaVictory" (rugido) se reproduce      │
│  ├─ Panel "VictoryPanel" aparece después de 1s         │
│  └─ Muestra: "¡VICTORIA! ¡Has derrotado al enemigo!"   │
└─────────────────────────────────────────────────────────┘
```

---

## ⚙️ Configuración Visual

### Estructura en Hierarchy:

```
Godzilla (Escena)
├─ Godzilla (modelo)
│  └─ GodzillaController (Script)
│
├─ Enemy
│  ├─ (modelo del enemigo)
│  ├─ Collider
│  └─ GodzillaEnemy (Script)
│
├─ PointA (Empty)
├─ PointB (Empty)
│
├─ GameManager (Empty)
│  └─ GodzillaGameManager (Script)
│
├─ SoundsController (Singleton)
│  └─ SoundsController (Script)
│
└─ Canvas
   └─ VictoryPanel (Panel - DESACTIVADO)
      └─ Text "¡VICTORIA!"
```

---

## 🎯 Tipos de Movimiento del Enemigo

### PingPong (Recomendado)
```
A ←→←→←→ B
  Ida y vuelta continuamente
```

### Loop
```
A ────→ B
  ↑      ↓
  └──────┘
  Ciclo continuo con teletransporte
```

---

## 🐛 Troubleshooting

### El enemigo no se mueve
- ✅ Verifica que PointA y PointB estén asignados
- ✅ Move Speed > 0
- ✅ Los puntos deben estar en posiciones diferentes

### El láser no detecta al enemigo
- ✅ El enemigo tiene Collider
- ✅ El Collider NO está en layer "Ignore Raycast"
- ✅ El enemigo tiene el script GodzillaEnemy

### El panel de victoria no aparece
- ✅ Victory Panel está asignado en GameManager
- ✅ Victory Panel está desactivado al inicio
- ✅ Godzilla Controller está asignado en GameManager

### El rugido no suena
- ✅ Audio "GodzillaVictory" está configurado en SoundsController
- ✅ Victory Roar Scene Name = "GodzillaVictory"
- ✅ O Victory Roar Audio tiene un AudioClipSO asignado

### El enemigo no se destruye
- ✅ Revisa la consola: debe decir "¡Láser impactó al enemigo: [nombre]!"
- ✅ El enemigo está en la línea del láser
- ✅ El Raycast alcanza al enemigo (aumenta Laser Max Distance)

---

## 💡 Tips y Mejoras

### Agregar Más Enemigos

Simplemente duplica el enemigo:
1. Selecciona el enemigo en Hierarchy
2. Ctrl+D para duplicar
3. Posiciónalo en otro lugar
4. Asígnale sus propios PointA y PointB

**El GameManager los detectará automáticamente** y requerirá destruirlos a TODOS para ganar.

### Ajustar Dificultad

```csharp
// Enemigo más rápido = más difícil
Move Speed: 5

// Enemigo más lento = más fácil
Move Speed: 1
```

### Efectos de Destrucción

En GodzillaEnemy:
```
Destruction Effect: [Arrastra Particle System]
Destruction Duration: 1.5
Destruction Sound: [AudioClipSO de explosión]
```

### Botones en el Panel de Victoria

Agrega botones al VictoryPanel:
1. Click derecho en VictoryPanel → UI → Button
2. Texto: "Reintentar" o "Menú Principal"
3. OnClick() → Arrastra GameManager
4. Función: `RestartGame()` o `ReturnToMenu()`

---

## ✅ Checklist de Configuración

- [ ] Enemigo tiene script GodzillaEnemy
- [ ] Enemigo tiene Collider
- [ ] PointA y PointB creados y posicionados
- [ ] PointA y PointB asignados en GodzillaEnemy
- [ ] GameManager creado con script GodzillaGameManager
- [ ] VictoryPanel creado y desactivado
- [ ] VictoryPanel y Text asignados en GameManager
- [ ] Godzilla Controller asignado en GameManager
- [ ] Audio "GodzillaVictory" configurado en SoundsController
- [ ] Probado en Play Mode:
  - [ ] Enemigo se mueve correctamente
  - [ ] Láser destruye al enemigo
  - [ ] Panel de victoria aparece
  - [ ] Rugido se reproduce

---

## 🎬 Resultado Final

1. ✅ Enemigo se mueve entre dos puntos
2. ✅ Godzilla rota y espera input
3. ✅ Presionas ESPACIO → Secuencia de 25s
4. ✅ Láser dispara en el segundo 16
5. ✅ Si el láser impacta al enemigo → Se destruye
6. ✅ Al terminar la secuencia (segundo 25):
   - Si enemigo fue destruido → ¡VICTORIA!
   - Rugido de Godzilla
   - Panel "¡GANASTE!" aparece
7. ✅ Si falló → Puede intentar de nuevo

¡Perfecto para un minijuego de Godzilla completo! 🦖⚡🏆
