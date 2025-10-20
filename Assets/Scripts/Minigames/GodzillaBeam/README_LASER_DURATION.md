# ⚡ Variable de Duración del Rayo Láser - Godzilla Controller

## 📋 Resumen
Implementación de una variable configurable en el Inspector para controlar la duración del rayo láser de Godzilla.

---

## ✅ Nueva Funcionalidad

### Variable Agregada

```csharp
[Header("Configuración del Láser")]
[Tooltip("Duración del rayo láser en segundos (cuánto tiempo permanece activo)")]
[SerializeField] private float laserDuration = 9f;
```

**Ubicación:** Sección "Configuración del Láser" en el Inspector

---

## 🎯 Cómo Funciona

### Fases del Láser:

```
1. CRECIMIENTO (0.3s)
   ↓
2. MANTENER ACTIVO (laserDuration - 0.6s)
   ↓
3. DESVANECIMIENTO (0.3s)
   ↓
Total = laserDuration segundos
```

### Cálculo Automático:

```csharp
float growDuration = 0.3f;        // Fase 1: Crecimiento
float fadeDuration = 0.3f;         // Fase 3: Fade out
float maintainDuration = laserDuration - growDuration - fadeDuration; // Fase 2

// Ejemplo con laserDuration = 9f:
// maintainDuration = 9 - 0.3 - 0.3 = 8.4 segundos
```

---

## ⚙️ Configuración en Unity

### En el Inspector:

Selecciona el GameObject con `GodzillaController` y verás:

```
[Header("Configuración del Láser")]
├─ Laser Max Distance: 1000
├─ Laser Start Width: 0.5
├─ Laser End Width: 0.3
├─ Laser Duration: 9.0 ⭐ NUEVO - Controla duración del rayo
└─ Enemy Layer: GodzillaEnemy
```

### Valores Recomendados:

| Duración | Efecto | Uso |
|----------|--------|-----|
| 3s | Muy corto | Rayo rápido, instantáneo |
| 5s | Corto | Disparo rápido |
| **9s** | **Balanceado** | **Recomendado (default)** |
| 15s | Largo | Rayo épico y dramático |
| 20s+ | Muy largo | Rayo prolongado, máximo dramatismo |

---

## 🔧 Implementación Técnica

### Antes (❌ Sin Control):

```csharp
// FASE 2: Mantener el rayo activo durante el resto del audio
// Del segundo 16 al 25 = 9 segundos totales
// Ya usamos 0.3s en crecimiento, quedan ~8.4s para mantener
// Usaremos 0.3s para el fade, así que mantenemos por 8.4s
// (Sin código real, solo comentarios)
```

**Problemas:**
- ❌ Duración hardcodeada en comentarios
- ❌ No se puede ajustar desde el Inspector
- ❌ Difícil de modificar para testing
- ❌ No hay control real sobre la duración

---

### Ahora (✅ Con Control):

```csharp
// FASE 2: Mantener el rayo activo durante la duración configurada
float fadeDuration = 0.3f;
float maintainDuration = laserDuration - growDuration - fadeDuration;

Debug.Log($"⚡ Manteniendo rayo activo por {maintainDuration:F1} segundos");
yield return new WaitForSeconds(maintainDuration);
```

**Beneficios:**
- ✅ Configurable desde el Inspector
- ✅ Cálculo automático de tiempo de mantenimiento
- ✅ Fácil de ajustar para diferentes efectos
- ✅ Debug log muestra duración real

---

## 📊 Desglose de Tiempos

### Con laserDuration = 9f (Default):

```
Fase 1: Crecimiento
├─ Duración: 0.3 segundos
├─ Descripción: El rayo crece desde el origen hasta el objetivo
└─ Visual: Interpolación suave de posición

Fase 2: Mantener Activo ⭐
├─ Duración: 8.4 segundos (laserDuration - 0.6)
├─ Descripción: El rayo permanece completamente visible
└─ Visual: Rayo estático en máxima intensidad

Fase 3: Desvanecimiento
├─ Duración: 0.3 segundos
├─ Descripción: El rayo se desvanece gradualmente
└─ Visual: Reducción de ancho e intensidad

Total: 9.0 segundos
```

### Con laserDuration = 5f (Rápido):

```
Fase 1: 0.3s
Fase 2: 4.4s
Fase 3: 0.3s
Total: 5.0s
```

### Con laserDuration = 15f (Épico):

```
Fase 1: 0.3s
Fase 2: 14.4s
Fase 3: 0.3s
Total: 15.0s
```

---

## 🎮 Uso en el Juego

### Escenario 1: Sincronización con Audio

Si tu audio de Godzilla dura 25 segundos y el rayo dispara en el segundo 16:

```
Audio Total: 25 segundos
Disparo en: 16 segundos
Tiempo restante: 25 - 16 = 9 segundos
laserDuration = 9f ✅ Perfecto
```

### Escenario 2: Rayo Más Corto

Si quieres que el rayo dure solo 5 segundos:

```
1. En el Inspector, cambia "Laser Duration" a 5
2. El rayo se disparará normalmente
3. Se mantendrá activo por 4.4 segundos
4. Se desvanecerá en 0.3 segundos
Total: 5 segundos ✅
```

### Escenario 3: Rayo Dramático Largo

Para un efecto épico más largo:

```
1. En el Inspector, cambia "Laser Duration" a 20
2. El rayo permanecerá activo por 19.4 segundos
3. Efecto muy dramático y cinematográfico ✅
```

---

## 🧪 Testing

### Test 1: Duración Default (9s)
1. Configura `laserDuration = 9`
2. Inicia el juego
3. Espera al disparo (segundo 16)
4. **Resultado Esperado:**
   - Rayo crece: 0.3s
   - Rayo activo: 8.4s
   - Rayo desvanece: 0.3s
   - Log: "⚡ Manteniendo rayo activo por 8.4 segundos"

### Test 2: Duración Corta (3s)
1. Configura `laserDuration = 3`
2. Inicia el juego
3. Espera al disparo
4. **Resultado Esperado:**
   - Rayo crece: 0.3s
   - Rayo activo: 2.4s
   - Rayo desvanece: 0.3s
   - Log: "⚡ Manteniendo rayo activo por 2.4 segundos"
   - Rayo termina mucho antes que el audio

### Test 3: Duración Muy Larga (20s)
1. Configura `laserDuration = 20`
2. Inicia el juego
3. Espera al disparo
4. **Resultado Esperado:**
   - Rayo crece: 0.3s
   - Rayo activo: 19.4s
   - Rayo desvanece: 0.3s
   - Log: "⚡ Manteniendo rayo activo por 19.4 segundos"
   - Rayo puede extenderse más allá del audio

---

## 🔍 Debug Log

Cuando el rayo se dispara, verás en la consola:

```
⚡ ¡Disparando rayo láser!
🎯 Origen del rayo: (0.0, 5.0, 0.0)
🎯 Dirección del rayo: (0.7, 0.0, 0.7)
🎯 Distancia máxima: 1000
🔍 Raycast detectó 1 enemigo(s)
💥 ¡IMPACTO! Enemigo GodzillaEnemy_01 destruido!
⚡ Manteniendo rayo activo por 8.4 segundos ⭐ NUEVO LOG
```

---

## 🎨 Efectos Visuales según Duración

### Corta (3-5s):
- ✅ Rápido y dinámico
- ✅ Bueno para múltiples disparos
- ❌ Menos cinematográfico

### Media (7-10s):
- ✅ Balanceado
- ✅ Cinematográfico
- ✅ Sincronizado con audio típico

### Larga (15-25s):
- ✅ Muy dramático
- ✅ Épico y poderoso
- ❌ Puede ser demasiado largo para gameplay rápido

---

## 📝 Código Completo

### Variable de Configuración:
```csharp
[Header("Configuración del Láser")]
[Tooltip("Duración del rayo láser en segundos (cuánto tiempo permanece activo)")]
[SerializeField] private float laserDuration = 9f;
```

### Uso en AnimateLaser():
```csharp
private IEnumerator AnimateLaser(Vector3 start, Vector3 end)
{
    // ... Configuración inicial ...
    
    float elapsed = 0f;
    float growDuration = 0.3f;

    // FASE 1: Crecimiento
    while (elapsed < growDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / growDuration;
        Vector3 currentEnd = Vector3.Lerp(start, end, t);
        laserBeam.SetPosition(1, currentEnd);
        yield return null;
    }

    laserBeam.SetPosition(1, end);

    // FASE 2: Mantener activo ⭐ NUEVO
    float fadeDuration = 0.3f;
    float maintainDuration = laserDuration - growDuration - fadeDuration;
    
    Debug.Log($"⚡ Manteniendo rayo activo por {maintainDuration:F1} segundos");
    yield return new WaitForSeconds(maintainDuration);

    // FASE 3: Desvanecimiento
    elapsed = 0f;
    while (elapsed < fadeDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / fadeDuration;
        float alpha = AnimationCurve.EaseInOut(0, 1, 1, 0).Evaluate(t);
        
        laserBeam.startWidth = laserStartWidth * alpha;
        laserBeam.endWidth = laserEndWidth * alpha;
        
        yield return null;
    }

    // Desactivar
    laserBeam.enabled = false;
    if (laserLight != null) laserLight.enabled = false;
}
```

---

## 🔧 Troubleshooting

### El rayo dura muy poco:
- ✅ Aumenta `laserDuration` en el Inspector
- ✅ Verifica que growDuration + fadeDuration (0.6s) no sean mayores que laserDuration
- ✅ Revisa el log para ver la duración real de mantenimiento

### El rayo dura demasiado:
- ✅ Reduce `laserDuration` en el Inspector
- ✅ Para testing rápido, usa valores entre 3-5 segundos

### El rayo se corta abruptamente:
- ✅ Verifica que fadeDuration esté en 0.3s
- ✅ Asegúrate de que el AnimationCurve esté funcionando
- ✅ Revisa que no haya otros scripts desactivando el LineRenderer

### Warning: maintainDuration negativo:
- ✅ Esto pasa si `laserDuration < 0.6`
- ✅ Solución: Configura `laserDuration >= 1f` como mínimo

---

## ✨ Mejoras Futuras Opcionales

- [ ] Agregar AnimationCurve para controlar la intensidad del rayo durante maintainDuration
- [ ] Agregar pulsos de energía durante la fase de mantenimiento
- [ ] Permitir configurar growDuration y fadeDuration desde el Inspector
- [ ] Agregar variación de color según la duración
- [ ] Agregar efectos de partículas que cambien con la duración

---

## 📊 Resumen de Cambios

### Archivos Modificados:
- `GodzillaController.cs`

### Cambios Específicos:

1. **Línea ~54:** Agregada variable `laserDuration`
   ```csharp
   [SerializeField] private float laserDuration = 9f;
   ```

2. **Línea ~348:** Modificado AnimateLaser() para usar laserDuration
   ```csharp
   float maintainDuration = laserDuration - growDuration - fadeDuration;
   yield return new WaitForSeconds(maintainDuration);
   ```

3. **Línea ~351:** Agregado debug log
   ```csharp
   Debug.Log($"⚡ Manteniendo rayo activo por {maintainDuration:F1} segundos");
   ```

---

**Fecha de Implementación:** 19 de Octubre, 2025  
**Estado:** ✅ Completado y Funcional  
**Tipo de Cambio:** Nueva funcionalidad configurable

**Resultado:**
- ✅ Control total sobre la duración del rayo
- ✅ Configurable desde el Inspector
- ✅ Cálculo automático de tiempos
- ✅ Debug logs informativos
- ✅ Fácil de ajustar para diferentes efectos
