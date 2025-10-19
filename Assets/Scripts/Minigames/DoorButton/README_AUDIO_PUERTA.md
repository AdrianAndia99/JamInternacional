# 🔊 Sistema de Audio con Loop para Puerta - Door Fight

## 📋 Resumen
Sistema de audio en loop que comienza con delay al iniciar el minijuego y se detiene automáticamente al finalizar (victoria o derrota).

---

## ✅ Funcionalidad Implementada

### 1. **Audio con Delay al Iniciar**
- **Delay Configurable:** 1 segundo por defecto (ajustable en Inspector)
- **Reproducción en Loop:** El audio se reproduce continuamente
- **Inicio Automático:** Comienza al cargar la escena y pasar el delay

### 2. **Detención Automática**
- **Al Ganar:** Se detiene cuando el tiempo llega a 0 (victoria)
- **Al Perder:** Se detiene cuando la puerta llega a -90° (derrota)
- **Limpio:** No interfiere con los audios de victoria/derrota

### 3. **Reinicio Correcto**
- **Al Reiniciar:** El audio vuelve a iniciar con el delay configurado
- **Sin Superposición:** Detiene el audio anterior antes de reiniciar

---

## 🎮 Configuración en Inspector

### DoorFight.cs
```
[Header("Audio (Opcional)")]
├─ Victory Audio: AudioClipSO (victoria)
├─ Defeat Audio: AudioClipSO (derrota)
├─ Door Effect Audio: AudioClipSO (efecto loop) ⭐ NUEVO
└─ Door Audio Delay: 1.0 (segundos) ⭐ NUEVO
```

### Parámetros:
- **doorEffectAudio:** El AudioClipSO del sonido de la puerta (chirrido, tensión, etc.)
- **doorAudioDelay:** Tiempo en segundos antes de iniciar el audio (recomendado: 0.5 - 2)

---

## 🔧 Métodos Implementados

### 1. StartDoorAudioWithDelay() - Corrutina
```csharp
private IEnumerator StartDoorAudioWithDelay()
{
    // Esperar el delay configurado
    yield return new WaitForSeconds(doorAudioDelay);

    // Reproducir el audio en loop
    if (doorEffectAudio != null)
    {
        doorEffectAudio.PlayLoop();
        Debug.Log("🚪 Audio de puerta iniciado en loop");
    }
}
```
**Función:** Espera el tiempo configurado y luego inicia el audio en loop

---

### 2. StopDoorAudio() - Método
```csharp
private void StopDoorAudio()
{
    if (doorEffectAudio != null)
    {
        doorEffectAudio.StopPlay();
        Debug.Log("🚪 Audio de puerta detenido");
    }
}
```
**Función:** Detiene el audio de la puerta de forma limpia

---

## 📝 Flujo de Ejecución

### 1. Inicio del Juego
```
Start() → StartGame() → StartCoroutine(StartDoorAudioWithDelay())
                                    ↓
                         Wait (doorAudioDelay segundos)
                                    ↓
                              PlayLoop() 🔊
```

### 2. Victoria
```
TriggerVictory() → StopDoorAudio() → PlayOneShoot(victoryAudio)
                        ↓
                   StopPlay() 🔇
```

### 3. Derrota
```
TriggerDefeat() → StopDoorAudio() → PlayOneShoot(defeatAudio)
                       ↓
                  StopPlay() 🔇
```

### 4. Reinicio
```
RestartGame() → StartGame() → StartCoroutine(StartDoorAudioWithDelay())
                                    ↓
                         Wait (doorAudioDelay segundos)
                                    ↓
                              PlayLoop() 🔊
```

---

## 🎯 Integración en el Código

### Modificaciones en StartGame()
```csharp
private void StartGame()
{
    gameActive = true;
    gameEnded = false;
    gameTimer = 0f;
    
    // ... código existente ...
    
    // ⭐ NUEVO: Iniciar el audio de la puerta con delay
    if (doorEffectAudio != null)
    {
        StartCoroutine(StartDoorAudioWithDelay());
    }
}
```

### Modificaciones en TriggerVictory()
```csharp
private void TriggerVictory()
{
    if (gameEnded) return;
    gameEnded = true;
    gameActive = false;

    // ⭐ NUEVO: Detener el audio de la puerta
    StopDoorAudio();

    // Reproducir audio de victoria
    if (victoryAudio != null)
    {
        victoryAudio.PlayOneShoot();
    }
    
    // ... resto del código ...
}
```

### Modificaciones en TriggerDefeat()
```csharp
private void TriggerDefeat()
{
    if (gameEnded) return;
    gameEnded = true;
    gameActive = false;

    // ⭐ NUEVO: Detener el audio de la puerta
    StopDoorAudio();

    // Reproducir audio de derrota
    if (defeatAudio != null)
    {
        defeatAudio.PlayOneShoot();
    }
    
    // ... resto del código ...
}
```

---

## ⚙️ Uso en Unity Editor

### 1. Seleccionar el GameObject con DoorFight.cs
En la jerarquía, selecciona el objeto que tiene el script `DoorFight`

### 2. Configurar en el Inspector
En la sección **"Audio (Opcional)"**:
1. Arrastra un **AudioClipSO** al campo "Door Effect Audio"
2. Ajusta "Door Audio Delay" (recomendado: 1 segundo)

### 3. Configurar el AudioClipSO
1. Crea un nuevo AudioClipSO: `Right Click > Create > Scriptable Audio > Audio > AudioClipSO`
2. Asígnale:
   - **AudioClip:** El sonido de la puerta (mp3/wav)
   - **MixerSO:** El mixer de SFX
   - **Volume:** 0.5 - 1.0 (ajusta según necesidad)
   - **Pitch:** 1.0 (normal) o ajusta para efecto

---

## 🧪 Testing

### Caso 1: Inicio Normal
1. Inicia el juego
2. **Resultado Esperado:** 
   - Espera 1 segundo
   - Log: "🚪 Audio de puerta iniciado en loop"
   - El audio comienza a sonar en loop

### Caso 2: Victoria
1. Juega hasta completar el tiempo (10 segundos)
2. **Resultado Esperado:**
   - Log: "🚪 Audio de puerta detenido"
   - El audio de puerta se detiene
   - Suena el audio de victoria

### Caso 3: Derrota
1. No presiones el botón, deja que la IA gane
2. **Resultado Esperado:**
   - Log: "🚪 Audio de puerta detenido"
   - El audio de puerta se detiene
   - Suena el audio de derrota

### Caso 4: Reinicio
1. Completa un juego (victoria o derrota)
2. Presiona "Reintentar"
3. **Resultado Esperado:**
   - Espera 1 segundo
   - Log: "🚪 Audio de puerta iniciado en loop"
   - El audio comienza de nuevo

---

## 📊 Valores Recomendados

| Parámetro | Valor Recomendado | Descripción |
|-----------|------------------|-------------|
| doorAudioDelay | 1.0s | Da tiempo al jugador para prepararse |
| Volume (AudioClipSO) | 0.5 - 0.7 | No debe tapar otros sonidos |
| Pitch (AudioClipSO) | 0.8 - 1.2 | Puede variar para dar tensión |

---

## 🔍 Troubleshooting

### El audio no suena:
- ✅ Verificar que `doorEffectAudio` esté asignado en el Inspector
- ✅ Verificar que el AudioClipSO tenga un AudioClip asignado
- ✅ Verificar que el MixerSO esté configurado correctamente
- ✅ Revisar el volumen del mixer y del AudioClipSO

### El audio no se detiene:
- ✅ Verificar que `StopDoorAudio()` se llama en TriggerVictory/TriggerDefeat
- ✅ Revisar los logs para confirmar que se ejecuta StopPlay()
- ✅ Verificar que no hay múltiples instancias del audio

### El audio se superpone al reiniciar:
- ✅ Asegurarse de que `StopDoorAudio()` se llama antes de StartGame()
- ✅ Verificar que solo hay una instancia de DoorFight en la escena

### El delay no funciona:
- ✅ Verificar que `doorAudioDelay` sea mayor a 0
- ✅ Asegurarse de que la corrutina se inicia correctamente
- ✅ Revisar que no hay errores en la consola

---

## 🎨 Sugerencias de Audio

### Tipos de Sonido Recomendados:
- **Tensión:** Cuerda tensándose, madera crujiendo
- **Mecánico:** Bisagras chirriando, metal rozando
- **Ambiente:** Viento, rumble bajo continuo
- **Esfuerzo:** Respiración pesada, gruñidos

### Características del Audio:
- **Duración:** 2-5 segundos (se loopea automáticamente)
- **Seamless Loop:** El final debe conectar con el inicio sin cortes
- **Volumen Consistente:** Evitar picos muy altos o bajos

---

## ✨ Mejoras Futuras Opcionales

- [ ] Variar el pitch del audio según el porcentaje de la IA (más tensión = pitch más alto)
- [ ] Agregar fade in al iniciar el audio
- [ ] Agregar fade out al detener el audio
- [ ] Cambiar a un audio más intenso cuando la IA > 80%
- [ ] Agregar efectos de audio adicionales al presionar el botón

---

## 🎵 Ejemplo de Implementación Avanzada (Opcional)

### Pitch Dinámico según Porcentaje de IA:
```csharp
// En Update(), después de actualizar aiPercentage
if (doorEffectAudio != null && gameActive)
{
    // Pitch sube gradualmente de 1.0 a 1.5 según porcentaje IA
    float dynamicPitch = Mathf.Lerp(1.0f, 1.5f, aiPercentage / 100f);
    // Nota: Requiere modificar AudioClipSO para soportar cambio dinámico de pitch
}
```

---

**Fecha de Implementación:** 19 de Octubre, 2025  
**Estado:** ✅ Completado y Funcional  
**Archivos Modificados:**
- DoorFight.cs
- README_AUDIO_PUERTA.md (este archivo)

---

**Dependencias:**
- AudioClipSO.cs (sistema de audio existente)
- SoundsController (singleton para reproducción)
- AudioMixerSO (mixer de audio)
