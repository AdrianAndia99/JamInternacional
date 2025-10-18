# 🦖 Nuevo Sistema de Godzilla - Disparo Único

## 📋 Cambios Implementados

### ✅ Nueva Lógica del Juego

**Antes:**
- Presionar SPACE para iniciar ataque
- Esperar 16 segundos para que dispare
- Esperar 25 segundos total para terminar
- Victoria solo si todos los enemigos están destruidos

**Ahora:**
- ✨ **La animación inicia automáticamente** al comenzar la escena
- ✨ Godzilla **rota continuamente** entre -40° y +40°
- ✨ **Presiona SPACE** para congelar la dirección
- ✨ **Disparo automático** después de `autoShootTime` segundos (default: 16s)
- ✨ **Solo 1 disparo** por partida
- ✨ **Victoria** si el disparo mata al enemigo
- ✨ **Derrota** si el disparo falla

---

## 🎮 Flujo del Juego

```
Inicio de Escena
    ↓
🎬 Animación activada automáticamente
🔊 Audio reproducido automáticamente
    ↓
🔄 Godzilla rota entre -40° y +40°
    ↓
🎯 [OPCIONAL] Presiona Q para verificar si apuntas al enemigo
    ↓
⏸️ Presiona SPACE para congelar dirección
    ↓
⏱️ Contador hasta autoShootTime (16s)
    ↓
⚡ DISPARO AUTOMÁTICO
    ↓
┌──────────────┬──────────────┐
│   💥 HIT     │   ❌ MISS    │
│   VICTORIA   │   DERROTA    │
└──────────────┴──────────────┘
    ↓               ↓
🎉 Panel       💀 Panel
   Victoria       Derrota
    ↓               ↓
🔊 Audio        🔊 Mismo Audio
```

---

## 🔧 Configuración en el Inspector

### GodzillaController

**Timing Configuration:**
```
Auto Shoot Time: 16
```
- Tiempo antes de disparar automáticamente
- Si el jugador NO presiona SPACE, dispara en la última dirección después de este tiempo

**Configuración del Láser:**
```
Laser Max Distance: 1000
Enemy Layer: GodzillaEnemy
```

---

### GodzillaGameManager

**Referencias UI:**
```
Victory Panel: [Asignar GameObject del panel de victoria]
Defeat Panel: [Asignar GameObject del panel de derrota]
Victory Text: [TextMeshPro del panel de victoria]
Defeat Text: [TextMeshPro del panel de derrota]
```

**Audio:**
```
Victory Roar Audio: [AudioClipSO del rugido]
```
- ⚠️ **IMPORTANTE**: El mismo audio se reproduce tanto en Victoria como en Derrota

---

## 🎯 Controles del Jugador

| Tecla | Acción |
|-------|--------|
| **Q** | Verificar si apuntas al enemigo (debug) |
| **SPACE** | Congelar dirección actual |

---

## 📊 Eventos y Resultados

### Victoria ✅
**Condiciones:**
- El Raycast detecta al enemigo
- El enemigo es destruido

**Acciones:**
1. `gameManager.TriggerVictory()`
2. Reproduce audio de rugido
3. Espera 1 segundo
4. Muestra panel de victoria

---

### Derrota ❌
**Condiciones:**
- El Raycast NO detecta ningún enemigo
- El disparo no impacta nada en el layer "GodzillaEnemy"

**Acciones:**
1. `gameManager.TriggerDefeat()`
2. Reproduce el MISMO audio de rugido
3. Espera 1 segundo
4. Muestra panel de derrota

---

## 🔍 Sistema de Debug

### Mensajes en Consola

**Al iniciar:**
```
🎬 Animación iniciada al comenzar la escena
🔊 Audio reproducido al inicio
```

**Al presionar Q:**
```
🎯 Verificando dirección actual:
   Ángulo: 15.3°
   Dirección: (0.26, 0.0, 0.97)
   ✅ ¡APUNTANDO A 1 ENEMIGO(S)! Presiona SPACE ahora para atacar
      - King Ghidorah (distancia: 45.2)
```

**Al presionar SPACE:**
```
🔒 Dirección congelada en: 15.3° - Dirección: (0.26, 0.0, 0.97)
⏱️ Tiempo restante para disparo: 10.5s
```

**Al disparar:**
```
⚡ ¡Disparando rayo láser!
🎯 Origen del rayo: (18.4, -1.39, -9.97)
🎯 Dirección del rayo: (0.26, 0.0, 0.97)
🔍 Raycast detectó 1 enemigo(s)
🎯 Raycast impactó: King Ghidorah - Distancia: 45.2m
💥 ¡IMPACTO! Enemigo King Ghidorah destruido!
✅ Enemigo King Ghidorah fue destruido!
🎉 ¡VICTORIA! Enemigo eliminado
Rugido de victoria reproducido!
```

---

## 🎨 UI Requerida

### Panel de Victoria
```
GameObject: VictoryPanel
├─ Image (fondo)
└─ TextMeshPro (VictoryText)
   └─ Texto: "¡VICTORIA!\n¡Has derrotado al enemigo!"
```

### Panel de Derrota
```
GameObject: DefeatPanel
├─ Image (fondo)
└─ TextMeshPro (DefeatText)
   └─ Texto: "¡DERROTA!\n¡Fallaste el disparo!"
```

### Botones (Opcionales)
- Botón "Reintentar" → `gameManager.RestartGame()`
- Botón "Menú" → `gameManager.ReturnToMenu()`

---

## ⚙️ Variables Eliminadas

Ya NO se usan las siguientes variables:
- ❌ `shootStartTime` (reemplazado por `autoShootTime`)
- ❌ `totalAudioDuration` (ya no se necesita)
- ❌ `isAttacking` (reemplazado por `laserFired`)
- ❌ `attackTimer` (reemplazado por `gameTimer`)
- ❌ `enemyDestroyedDuringAttack` (lógica simplificada)

---

## 🚀 Métodos Nuevos

### GodzillaController
```csharp
LockDirection()          // Congela la dirección actual
ShowResult(bool, float)  // Muestra victoria o derrota después de un delay
```

### GodzillaGameManager
```csharp
TriggerVictory()   // Activa secuencia de victoria
TriggerDefeat()    // Activa secuencia de derrota
ShowDefeatPanel()  // Muestra panel de derrota
```

---

## 📝 Métodos Eliminados

### GodzillaController
```csharp
StartAttackSequence()    // Ya no se necesita
HandleAttackSequence()   // Ya no se necesita
FinishAttackSequence()   // Ya no se necesita
TestFireLaser()          // Método de debug eliminado
```

---

## 🎯 Testing Rápido

1. **Play** → La animación y audio deben iniciarse automáticamente
2. **Espera** → Godzilla rota automáticamente
3. **Presiona Q** varias veces → Verifica cuándo apuntas al enemigo
4. **Presiona SPACE** cuando veas "✅ APUNTANDO" → Congela dirección
5. **Espera 16s** → Disparo automático
6. **Verifica resultado** → Panel de Victoria o Derrota

---

## ⚠️ Notas Importantes

1. **El audio se reproduce AL INICIO**, no al presionar SPACE
2. **Solo hay 1 disparo** por partida
3. **El mismo audio** se usa para Victoria y Derrota
4. **Auto Shoot Time** determina cuándo dispara automáticamente
5. **Layer "GodzillaEnemy"** DEBE estar configurado correctamente

---

## 🐛 Troubleshooting

### El disparo no detecta al enemigo
1. Verifica que el enemigo tenga Layer "GodzillaEnemy"
2. Presiona Q para ver si apuntas correctamente
3. Asegúrate de presionar SPACE cuando dice "✅ APUNTANDO"

### El audio no suena
1. Verifica que `godzillaRayoDisparoAudio` esté asignado en GodzillaController
2. Verifica que `victoryRoarAudio` esté asignado en GodzillaGameManager

### No aparece el panel
1. Verifica que `victoryPanel` y `defeatPanel` estén asignados
2. Verifica que estén desactivados al inicio
3. Revisa la consola para ver si se llama `TriggerVictory()` o `TriggerDefeat()`

---

¡Sistema simplificado y más ágil! 🎮✨
