# 🦘 Sistema de Salto - Dino Runner

## 📋 Resumen
Sistema de salto implementado para esquivar obstáculos en el minijuego Dino Runner.

---

## ✅ Funcionalidad Implementada

### 1. **Detección de Input**
- **Tecla:** ESPACIO (Space)
- **Condición:** Solo permite saltar si el jugador no está ya saltando
- **Método:** `DetectJump()` en Update()

### 2. **Animación de Salto**
- **Duración:** 0.5 segundos (configurable)
- **Altura:** 2 unidades (configurable)
- **Fases:**
  - **Subida:** 0.25s con interpolación suave
  - **Bajada:** 0.25s con interpolación suave
- **Implementación:** Corrutina `JumpCoroutine()` usando `Mathf.Lerp`

### 3. **Detección de Obstáculos**
- **Verificación de Altura:** Al colisionar, se compara la posición Y del jugador vs el obstáculo
- **Altura Mínima:** 1 unidad por encima del obstáculo para esquivarlo
- **Resultado:**
  - ✅ Si `playerY > obstacleY + 1f` → Obstáculo esquivado
  - ❌ Si no → El jugador recibe el golpe

---

## 🎮 Configuración en Inspector

### DinoRunner.cs
```
[Header("Sistema de Salto")]
├─ Jump Height: 2
├─ Jump Duration: 0.5
└─ Jump Animation Name: "Jump" (opcional)
```

### Parámetros Ajustables:
- **jumpHeight:** Altura del salto (recomendado: 2-3)
- **jumpDuration:** Duración total del salto (recomendado: 0.4-0.6)
- **jumpAnimationName:** Nombre de la animación de salto (dejar vacío si no hay)

---

## 📝 Variables de Estado

### Privadas
```csharp
private bool isJumping = false;        // Indica si está saltando
private float playerStartY;            // Posición Y inicial del jugador
```

### Inicialización
En `Start()`:
```csharp
playerStartY = player.position.y;     // Guardar Y inicial
```

---

## 🔧 Métodos Implementados

### 1. DetectJump()
```csharp
private void DetectJump()
{
    if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
    {
        StartCoroutine(JumpCoroutine());
    }
}
```
**Función:** Detecta cuando el jugador presiona ESPACIO y no está saltando

---

### 2. JumpCoroutine()
```csharp
private IEnumerator JumpCoroutine()
{
    isJumping = true;
    
    // Reproducir animación si existe
    if (playerAnimator != null && !string.IsNullOrEmpty(jumpAnimationName))
    {
        playerAnimator.Play(jumpAnimationName);
    }

    // FASE DE SUBIDA
    float halfDuration = jumpDuration / 2f;
    for (float t = 0; t < halfDuration; t += Time.deltaTime)
    {
        float progress = t / halfDuration;
        float newY = Mathf.Lerp(playerStartY, playerStartY + jumpHeight, progress);
        player.position = new Vector3(player.position.x, newY, player.position.z);
        yield return null;
    }
    
    // FASE DE BAJADA
    for (float t = 0; t < halfDuration; t += Time.deltaTime)
    {
        float progress = t / halfDuration;
        float newY = Mathf.Lerp(playerStartY + jumpHeight, playerStartY, progress);
        player.position = new Vector3(player.position.x, newY, player.position.z);
        yield return null;
    }
    
    player.position = new Vector3(player.position.x, playerStartY, player.position.z);
    isJumping = false;
}
```
**Función:** Anima el salto del jugador de forma suave

---

### 3. ObstacleMovement - Detección de Colisión Mejorada

#### Para Collider3D:
```csharp
private void OnTriggerEnter(Collider other)
{
    if (hasHit || !other.CompareTag("Player")) return;
    
    float playerY = other.transform.position.y;
    float obstacleY = transform.position.y;
    float clearanceHeight = 1f;
    
    if (playerY > obstacleY + clearanceHeight)
    {
        // ✅ Obstáculo esquivado
        Debug.Log("🦘 ¡Obstáculo esquivado con salto!");
        hasHit = true;
        Destroy(gameObject);
        return;
    }
    
    // ❌ Golpe recibido
    hasHit = true;
    gameManager.OnObstacleHit();
    Destroy(gameObject);
}
```

#### Para Collider2D:
```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    // Misma lógica que OnTriggerEnter pero con Collider2D
}
```

---

## 🎯 Cómo Usar

### 1. En Unity Editor:
1. Selecciona el GameObject con `DinoRunner.cs`
2. En el Inspector, configura:
   - Jump Height: 2
   - Jump Duration: 0.5
   - Jump Animation Name: "Jump" (si tienes animación)

### 2. En el Juego:
- **A / D:** Correr alternadamente
- **ESPACIO:** Saltar para esquivar obstáculos

### 3. Debugging:
Los logs muestran:
- `🦘 Salto completado` - Cuando termina el salto
- `🦘 ¡Obstáculo esquivado con salto!` - Cuando se esquiva exitosamente
- `💥 Obstáculo golpeó al jugador` - Cuando recibe el golpe

---

## ⚙️ Requisitos Técnicos

### GameObject del Jugador:
- ✅ Tag: "Player"
- ✅ Collider (2D o 3D) con "Is Trigger" habilitado
- ✅ Transform asignado en DinoRunner

### GameObject del Obstáculo:
- ✅ Collider (2D o 3D) con "Is Trigger" habilitado
- ✅ Script ObstacleMovement.cs
- ✅ Inicializado con `Initialize(player, speed, dinoRunner)`

---

## 🧪 Testing

### Caso 1: Salto Exitoso
1. Esperar que aparezca un obstáculo
2. Presionar ESPACIO cuando el obstáculo esté cerca
3. **Resultado Esperado:** Log "🦘 ¡Obstáculo esquivado con salto!"

### Caso 2: Salto Tardío
1. Presionar ESPACIO cuando el obstáculo ya pasó
2. **Resultado Esperado:** Log "💥 Obstáculo golpeó al jugador"

### Caso 3: Sin Saltar
1. No presionar ESPACIO
2. **Resultado Esperado:** Log "💥 Obstáculo golpeó al jugador"

---

## 📊 Valores Recomendados

| Parámetro | Valor Recomendado | Descripción |
|-----------|------------------|-------------|
| jumpHeight | 2f | Altura suficiente para pasar obstáculos de 1m |
| jumpDuration | 0.5f | Salto rápido pero controlable |
| clearanceHeight | 1f | Altura del obstáculo a superar |

---

## 🔍 Troubleshooting

### El jugador no salta:
- ✅ Verificar que `player` no sea null
- ✅ Verificar que `playerStartY` esté inicializado en Start()
- ✅ Verificar que `using System.Collections;` esté en los imports

### El salto no esquiva obstáculos:
- ✅ Verificar que jumpHeight > clearanceHeight (recomendado: jumpHeight = 2, clearanceHeight = 1)
- ✅ Verificar que los colliders sean "Is Trigger"
- ✅ Verificar que el jugador tenga el tag "Player"

### Los obstáculos siempre golpean:
- ✅ Verificar los logs para ver la posición Y del jugador vs obstáculo
- ✅ Aumentar jumpHeight si es necesario
- ✅ Verificar que el timing del salto sea correcto

---

## ✨ Características Destacadas

1. **Interpolación Suave:** Usa `Mathf.Lerp` para movimiento fluido
2. **Seguridad de Estado:** `isJumping` previene saltos múltiples
3. **Soporte 2D y 3D:** Funciona con Collider2D y Collider
4. **Animación Opcional:** Soporta animaciones si están disponibles
5. **Debug Completo:** Logs descriptivos para facilitar testing

---

## 🚀 Próximas Mejoras Opcionales

- [ ] Agregar partículas al saltar
- [ ] Agregar sonido de salto
- [ ] Implementar salto doble
- [ ] Agregar trail effect al saltar
- [ ] Implementar curva de salto personalizada (AnimationCurve)

---

**Fecha de Implementación:** 2025  
**Estado:** ✅ Completado y Funcional  
**Archivos Modificados:**
- DinoRunner.cs
- ObstacleMovement.cs
