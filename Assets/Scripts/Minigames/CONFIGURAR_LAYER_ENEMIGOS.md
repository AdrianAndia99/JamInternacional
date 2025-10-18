# 🎯 Guía: Configurar Layer de Enemigos

## ✅ Paso 1: Crear el Layer "GodzillaEnemy"

1. En Unity, ve al menú superior:
   ```
   Edit > Project Settings > Tags and Layers
   ```

2. En la ventana que se abre:
   - Busca la sección **"Layers"**
   - Encuentra el primer slot vacío (probablemente "User Layer 6", "User Layer 7", etc.)
   - Haz click en el slot vacío
   - Escribe: **`GodzillaEnemy`**
   - Presiona Enter

3. Cierra la ventana de Project Settings

**Resultado esperado:**
```
Layers:
├─ Default
├─ TransparentFX
├─ Ignore Raycast
├─ ...
├─ User Layer 6: GodzillaEnemy  ← NUEVO
```

---

## ✅ Paso 2: Asignar el Layer al Enemigo "King Ghidorah"

1. En la **Hierarchy**, selecciona **"King Ghidorah"**

2. En el **Inspector**, arriba a la derecha, busca:
   ```
   Tag: Untagged    Layer: Default ▼
                           ↑ HAZ CLICK AQUÍ
   ```

3. En el dropdown que aparece, selecciona **"GodzillaEnemy"**

4. Unity preguntará:
   ```
   "Do you want to set layer to all child objects?"
   ```
   - Haz click en **"Yes, change children"**

**Resultado esperado:**
```
King Ghidorah
├─ Layer: GodzillaEnemy  ← CAMBIADO
└─ Todos los hijos también tendrán "GodzillaEnemy"
```

---

## ✅ Paso 3: Configurar el GodzillaController en el Inspector

1. En la **Hierarchy**, selecciona el objeto que tiene **GodzillaController** (probablemente "Godzilla")

2. En el **Inspector**, busca la sección:
   ```
   Godzilla Controller (Script)
   ├─ ...
   └─ Configuración del Láser
      ├─ Laser Max Distance: 1000
      ├─ Laser Start Width: 0.5
      ├─ Laser End Width: 0.3
      └─ Enemy Layer: Nothing ▼  ← HAZ CLICK AQUÍ
   ```

3. Haz click en **"Enemy Layer"** y selecciona **SOLO "GodzillaEnemy"**:
   - Asegúrate de que SOLO esté marcado "GodzillaEnemy"
   - Desmarca "Default", "Everything", etc.

**Resultado esperado:**
```
Enemy Layer: GodzillaEnemy
```

---

## ✅ Paso 4: Verificar la Configuración

### Verificar el Enemigo:
1. Selecciona **"King Ghidorah"**
2. En Inspector, verifica:
   - ✅ **Layer: GodzillaEnemy**
   - ✅ **Capsule Collider** (o Box/Sphere Collider)
   - ✅ **Is Trigger: DESACTIVADO** (el checkbox debe estar vacío)
   - ✅ **GodzillaEnemy (Script)** presente

### Verificar el Controller:
1. Selecciona el objeto con **GodzillaController**
2. En Inspector, verifica:
   - ✅ **Laser Origin**: Asignado (Boca-Mouth)
   - ✅ **Laser Max Distance**: 1000 (o mayor)
   - ✅ **Enemy Layer**: Solo "GodzillaEnemy" seleccionado

---

## 🎮 Paso 5: Probar el Sistema

1. **Presiona Play**
2. **Presiona SPACE** cuando Godzilla esté apuntando al enemigo
3. **Espera 16 segundos**
4. **Mira la CONSOLA**

### Mensajes esperados (SI FUNCIONA):
```
¡Disparando rayo láser!
🎯 Origen del rayo: (18.4, -1.39, -9.97)
🎯 Dirección del rayo: (0.5, 0.0, 0.87)
🎯 Distancia máxima: 1000
🎯 Layer configurado: GodzillaEnemy
🔍 Raycast detectó 1 objetos en el layer de enemigos
🎯 Raycast impactó: King Ghidorah (Layer: GodzillaEnemy) - Distancia: 45.2
✅ ¡Láser impactó al enemigo: King Ghidorah!
💥 Enemigo King Ghidorah destruido por el láser!
✅ GameManager notificado de la destrucción de King Ghidorah
```

### Mensajes si NO funciona:
```
🔍 Raycast detectó 0 objetos en el layer de enemigos
⚠️ El Raycast no detectó NINGÚN enemigo. Posibles causas:
   1. El enemigo NO está en el layer configurado: GodzillaEnemy
   ...
```

**Si sale "0 objetos":**
- Verifica que King Ghidorah tenga Layer "GodzillaEnemy"
- Verifica que en GodzillaController, Enemy Layer esté configurado
- Verifica que el Capsule Collider NO sea Trigger

---

## 🔧 Troubleshooting

### Problema: "Layer configurado: Mixed" o algo raro
**Solución:**
1. Selecciona GodzillaController
2. En Enemy Layer, haz click
3. Primero haz click en "Nothing" (deseleccionar todo)
4. Luego marca SOLO "GodzillaEnemy"

---

### Problema: "Raycast detectó 0 objetos"
**Verificar:**
1. King Ghidorah tiene Layer "GodzillaEnemy" ✓
2. GodzillaController tiene Enemy Layer = "GodzillaEnemy" ✓
3. Capsule Collider "Is Trigger" está DESACTIVADO ✓
4. GodzillaEnemy script está en el GameObject raíz ✓

---

### Problema: "Objeto está en layer pero NO tiene GodzillaEnemy component"
**Solución:**
- El script GodzillaEnemy debe estar en el MISMO GameObject que tiene el Capsule Collider
- Si está en un hijo, muévelo al padre

**Estructura correcta:**
```
King Ghidorah (GameObject raíz)
├─ Layer: GodzillaEnemy
├─ Capsule Collider
├─ GodzillaEnemy (Script) ← AQUÍ
└─ Ghidorah (modelo hijo)
   └─ Animator
```

---

## 📊 Ventajas de Usar un Layer Específico

✅ **Rendimiento**: El Raycast solo busca en objetos del layer GodzillaEnemy, no en todo el escenario
✅ **Precisión**: No detectará objetos incorrectos (paredes, suelo, etc.)
✅ **Claridad**: Fácil saber qué objetos puede golpear el láser
✅ **Debugging**: Mensajes más claros sobre qué se detectó

---

## 🎯 Resumen Rápido

```
1. Edit > Project Settings > Tags and Layers
   └─ Crear layer "GodzillaEnemy"

2. Seleccionar King Ghidorah
   └─ Layer: GodzillaEnemy

3. Seleccionar Godzilla (GodzillaController)
   └─ Enemy Layer: GodzillaEnemy

4. Play > Space > Esperar 16s

5. ¡BOOM! 💥 Enemigo destruido
```

---

¡Con esta configuración, el láser SOLO detectará objetos en el layer "GodzillaEnemy"! 🦖⚡
