# 🐛 Guía de Troubleshooting - Enemigo no se Destruye

## ✅ Checklist de Verificación

### 1️⃣ Verificar el Collider del Enemigo

Selecciona el enemigo "King Ghidorah" en Hierarchy y verifica:

```
Inspector del Enemigo:
├─ ✅ Tiene un Collider (Capsule Collider en la imagen)
│  ├─ Is Trigger: ☐ DEBE ESTAR DESACTIVADO
│  ├─ Center: Ajustado al centro del modelo
│  ├─ Radius: 782.9681
│  └─ Height: 2428.875
│
├─ ✅ GodzillaEnemy (Script) está agregado
│  ├─ Point A: Asignado
│  ├─ Point B: Asignado
│  └─ Move Speed: 20
│
└─ Layer: ⚠️ NO debe estar en "Ignore Raycast"
```

### 2️⃣ Verificar el Layer del Enemigo

**IMPORTANTE**: El enemigo NO debe estar en layer "Ignore Raycast"

1. Selecciona el enemigo
2. En el Inspector, arriba, busca "Layer"
3. Debe estar en "Default" o cualquier layer EXCEPTO "Ignore Raycast"

### 3️⃣ Verificar el Laser Origin de Godzilla

En GodzillaController:
- **Laser Origin**: Debe apuntar al Transform "Boca-Mouth"
- **Laser Max Distance**: 1000 (debería ser suficiente)

### 4️⃣ Prueba con Debugging Mejorado

Ahora cuando dispares el láser, verás mensajes detallados en la consola:

```
Al presionar ESPACIO (segundo 16):
├─ "¡Disparando rayo láser!"
├─ "🎯 Origen del rayo: (X, Y, Z)"
├─ "🎯 Dirección del rayo: (X, Y, Z)"
├─ "🎯 Distancia máxima: 1000"
├─ "🔍 Raycast detectó X objetos"
│
└─ Para cada objeto detectado:
   ├─ "🎯 Raycast impactó: [nombre] (Layer: [layer])"
   │
   ├─ SI ES ENEMIGO:
   │  └─ "✅ ¡Láser impactó al enemigo: [nombre]!"
   │     └─ "💥 Enemigo [nombre] destruido por el láser!"
   │        └─ "✅ GameManager notificado de la destrucción"
   │
   └─ SI NO ES ENEMIGO:
      └─ "ℹ️ Objeto [nombre] no es un enemigo"
```

---

## 🔧 Soluciones Paso a Paso

### Problema 1: "🔍 Raycast detectó 0 objetos"

**Causas posibles:**
- El enemigo está fuera del alcance
- El enemigo está en layer "Ignore Raycast"
- La dirección del láser está incorrecta

**Solución:**
1. Aumenta `Laser Max Distance` a 5000
2. Verifica que el enemigo no esté en "Ignore Raycast"
3. En Play Mode, observa el Gizmo rojo que muestra la dirección del rayo

---

### Problema 2: Raycast detecta objetos pero no al enemigo

**Mensaje en consola:**
```
🔍 Raycast detectó 2 objetos
🎯 Raycast impactó: Godzilla (Layer: Default)
🎯 Raycast impactó: Ground (Layer: Default)
ℹ️ Objeto Godzilla no es un enemigo
ℹ️ Objeto Ground no es un enemigo
```

**Causa**: El Raycast no está llegando al enemigo

**Solución:**
1. Verifica que el enemigo esté en la trayectoria del láser
2. Usa el Gizmo rojo en Scene View para ver la dirección
3. Mueve al enemigo más cerca de la línea del láser

---

### Problema 3: Raycast detecta al enemigo pero no lo destruye

**Mensaje en consola:**
```
🎯 Raycast impactó: King Ghidorah (Layer: Default)
ℹ️ Objeto King Ghidorah no es un enemigo (no tiene GodzillaEnemy component)
```

**Causa**: El script `GodzillaEnemy` no está en el GameObject correcto

**Solución:**
1. El enemigo tiene el modelo visual como hijo
2. Asegúrate de agregar `GodzillaEnemy` al GameObject RAÍZ (el que tiene el Collider)
3. NO al modelo hijo "Ghidorah" dentro de "King Ghidorah"

**Estructura correcta:**
```
King Ghidorah (GameObject raíz) ← GodzillaEnemy.cs AQUÍ
├─ Capsule Collider ← Collider AQUÍ
└─ Ghidorah (modelo visual hijo)
   └─ Animator
```

---

### Problema 4: El enemigo se destruye pero no hay victoria

**Mensaje en consola:**
```
💥 Enemigo King Ghidorah destruido por el láser!
⚠️ GameManager es null! No se puede notificar la destrucción.
```

**Causa**: El GameManager no está en la escena o no se encontró

**Solución:**
1. Verifica que existe un GameObject "GameManager" en la escena
2. Verifica que tiene el script `GodzillaGameManager`
3. El enemigo lo buscará automáticamente en `Start()`

---

## 🎯 Prueba Rápida

1. **Presiona Play**
2. **Presiona ESPACIO** (Godzilla se detiene)
3. **Espera 16 segundos** (el láser dispara)
4. **Mira la CONSOLA** y busca:
   - ¿Dice "🔍 Raycast detectó 0 objetos"? → El enemigo no está en la trayectoria
   - ¿Dice "Raycast impactó: King Ghidorah"? → ¡Bien!
   - ¿Dice "no tiene GodzillaEnemy component"? → Agrega el script al GameObject correcto
   - ¿Dice "💥 Enemigo destruido"? → ¡PERFECTO!

---

## 📊 Información de Debug Esperada (Funcionando Correctamente)

```
Al segundo 16:
─────────────────────────────────────────────────
¡Disparando rayo láser!
🎯 Origen del rayo: (18.405, -1.39, -9.97)
🎯 Dirección del rayo: (0.5, 0.0, 0.87)
🎯 Distancia máxima: 1000
🔍 Raycast detectó 1 objetos
🎯 Raycast impactó: King Ghidorah (Layer: Default)
✅ ¡Láser impactó al enemigo: King Ghidorah!
💥 Enemigo King Ghidorah destruido por el láser!
✅ GameManager notificado de la destrucción de King Ghidorah
🗑️ GameObject King Ghidorah destruido completamente
─────────────────────────────────────────────────
Al segundo 25:
─────────────────────────────────────────────────
Secuencia de ataque completada. Enemigo destruido: True
¡VICTORIA! Todos los enemigos destruidos.
Rugido de victoria reproducido!
─────────────────────────────────────────────────
```

---

## 🔍 Visualización en Scene View

Cuando el láser dispara, verás:
- **Gizmo rojo**: Línea desde la boca de Godzilla en la dirección del disparo
- **Esfera roja**: En el origen del láser (boca)
- **Línea cyan**: Si el láser está activo

Usa esto para verificar visualmente que el rayo apunta al enemigo.

---

## ⚡ Solución Rápida si Nada Funciona

Si después de todo esto no funciona:

1. **Selecciona el enemigo** (King Ghidorah)
2. **En el Inspector**, haz click en los 3 puntos arriba a la derecha
3. **Debug** → Copia y pega el layer y nombre exactos
4. **Verifica que GodzillaEnemy script** esté en el mismo GameObject que el Collider

O simplemente:
1. Crea un **Cube** simple
2. Ponle **GodzillaEnemy** script
3. Asígnale PointA y PointB
4. Prueba si se destruye con el láser
5. Si funciona, el problema es específico del modelo "King Ghidorah"

---

¡Prueba con el debugging mejorado y dime qué mensajes ves en la consola! 🦖⚡
