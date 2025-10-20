# 🎨 Sistema de Parallax Background - Dino Runner

## 📋 Resumen
Sistema de parallax para crear efecto de profundidad con múltiples capas de fondo que se mueven a diferentes velocidades.

---

## ✨ Características

- ✅ Soporte para múltiples capas (2 o más)
- ✅ Velocidad independiente por capa
- ✅ Loop infinito automático
- ✅ Configurable desde el Inspector
- ✅ Control de velocidad global
- ✅ Pausar/reanudar movimiento

---

## 🎮 Cómo Funciona

### Efecto Parallax:

```
Capa Lejana (Cielo/Montañas)
├─ Velocidad: 0.5x
└─ Se mueve lento = Parece lejano

Capa Cercana (Suelo/Rocas)
├─ Velocidad: 1.5x
└─ Se mueve rápido = Parece cercano
```

**Resultado:** Sensación de profundidad y movimiento 3D

---

## ⚙️ Setup en Unity (Paso a Paso)

### 1️⃣ Preparar tus Imágenes de Fondo

**Requisitos:**
- 2 sprites de fondo (ejemplo: cielo + suelo)
- Ambos deben ser **suficientemente anchos** para cubrir la pantalla
- Recomendado: Ancho = 2x el ancho de pantalla para loop suave

**Ejemplo:**
```
Fondo_Cielo.png (Resolución: 1920x1080)
Fondo_Suelo.png (Resolución: 1920x1080)
```

---

### 2️⃣ Crear GameObject ParallaxManager

1. **Crear GameObject vacío:**
   - Hierarchy → Right Click → Create Empty
   - Nombra: `ParallaxManager`

2. **Agregar el Script:**
   - Con `ParallaxManager` seleccionado
   - Inspector → Add Component
   - Busca: `ParallaxBackground`
   - Agrega el script

---

### 3️⃣ Configurar las Capas de Fondo

#### **Capa 0: Fondo Lejano (Cielo/Montañas)**

1. **Crear el GameObject:**
   - Hierarchy → Right Click → 2D Object → Sprite
   - Nombra: `Fondo_InferiorFondo` o `BackgroundLayer_0`

2. **Asignar el Sprite:**
   - Inspector → Sprite Renderer → Sprite
   - Arrastra tu imagen de fondo lejano

3. **Posicionar:**
   - Transform Position: `(0, 0, 10)` (Z=10 para que esté atrás)
   - Scale: Ajusta para cubrir la pantalla

4. **Duplicar para Loop:**
   - Duplica el GameObject (Ctrl+D)
   - Nombra: `Fondo_InferiorFondo_Copy`
   - Position: `(ancho_sprite, 0, 10)` 
   - Ejemplo: Si el sprite tiene 20 unidades de ancho → `(20, 0, 10)`

---

#### **Capa 1: Fondo Cercano (Suelo/Rocas)**

1. **Crear el GameObject:**
   - Hierarchy → Right Click → 2D Object → Sprite
   - Nombra: `InferiorFondo` o `BackgroundLayer_1`

2. **Asignar el Sprite:**
   - Inspector → Sprite Renderer → Sprite
   - Arrastra tu imagen de fondo cercano

3. **Posicionar:**
   - Transform Position: `(0, 0, 5)` (Z=5 para que esté adelante del cielo)
   - Scale: Ajusta para cubrir la pantalla

4. **Duplicar para Loop:**
   - Duplica el GameObject (Ctrl+D)
   - Nombra: `InferiorFondo_Copy`
   - Position: `(ancho_sprite, 0, 5)`

---

### 4️⃣ Configurar el ParallaxBackground Script

Selecciona `ParallaxManager` en la Hierarchy:

```
[Header("Capas de Parallax")]
Layers:
  └─ Size: 2
     
     Element 0 (Capa Lejana):
     ├─ Layer Transform: [Arrastra Fondo_InferiorFondo]
     ├─ Parallax Speed: 0.5
     └─ Sprite Width: 20 (ancho de tu sprite)
     
     Element 1 (Capa Cercana):
     ├─ Layer Transform: [Arrastra InferiorFondo]
     ├─ Parallax Speed: 1.5
     └─ Sprite Width: 20

[Header("Configuración Global")]
├─ Base Speed: 2
└─ Auto Move: ✓ (checked)
```

---

## 🎯 Valores Recomendados

### Para 2 Capas:

| Capa | Nombre | Parallax Speed | Z Position | Uso |
|------|--------|----------------|------------|-----|
| 0 | Cielo/Montañas | 0.3 - 0.5 | 10 | Muy lejano |
| 1 | Suelo/Rocas | 1.0 - 1.5 | 5 | Cercano |

### Velocidad Base:

| Base Speed | Efecto |
|------------|--------|
| 1.0 | Lento, relajado |
| 2.0 | Balanceado (recomendado) |
| 3.0 | Rápido, dinámico |
| 5.0 | Muy rápido |

---

## 🔧 Cómo Calcular el Sprite Width

### Método 1: En Unity Editor

1. Selecciona tu sprite en la Hierarchy
2. Inspector → Transform → Scale
3. Si Scale X = 1:
   ```
   Sprite Width = Bounds.size.x del Sprite Renderer
   ```
4. Si Scale X ≠ 1:
   ```
   Sprite Width = Bounds.size.x * Scale.x
   ```

### Método 2: Medición Manual

1. En Scene View, selecciona el sprite
2. Mira las dimensiones en el Gizmo
3. Anota el ancho en unidades de Unity

### Ejemplo:
```
Si tu sprite ocupa desde X=0 hasta X=20:
Sprite Width = 20
```

---

## 🎨 Configuración Visual

### Estructura de la Jerarquía:

```
RunDino (Escena)
├─ ParallaxManager [ParallaxBackground.cs]
├─ BackgroundLayers
│  ├─ Fondo_InferiorFondo (Z=10)
│  ├─ Fondo_InferiorFondo_Copy (Z=10, X=20)
│  ├─ InferiorFondo (Z=5)
│  └─ InferiorFondo_Copy (Z=5, X=20)
├─ Player
└─ Dinosaurio
```

---

## 📝 Configuración Completa Ejemplo

### Inspector del ParallaxManager:

```
[ParallaxBackground]

Capas de Parallax:
  Layers: Array[2]
  
  ┌─ Element 0 (Capa Cielo)
  │  ├─ Layer Transform: Fondo_InferiorFondo
  │  ├─ Parallax Speed: 0.5
  │  └─ Sprite Width: 20
  │
  └─ Element 1 (Capa Suelo)
     ├─ Layer Transform: InferiorFondo
     ├─ Parallax Speed: 1.5
     └─ Sprite Width: 20

Configuración Global:
  ├─ Base Speed: 2
  └─ Auto Move: ✓

Referencias (Opcional):
  └─ Dino Runner: [Vacío o asigna si quieres]
```

---

## 🧪 Testing

### Test 1: Movimiento Básico
1. Dale Play en Unity
2. **Resultado Esperado:**
   - Ambas capas se mueven a la izquierda
   - La capa cercana (suelo) se mueve más rápido
   - La capa lejana (cielo) se mueve más lento
   - Efecto de profundidad visible

### Test 2: Loop Infinito
1. Deja el juego corriendo por 30 segundos
2. **Resultado Esperado:**
   - Las capas se repiten sin cortes
   - No hay espacios vacíos
   - El movimiento es continuo

### Test 3: Diferentes Velocidades
1. Cambia `Base Speed` a 5
2. **Resultado Esperado:**
   - Todo se mueve más rápido
   - La diferencia de velocidad entre capas se mantiene

---

## 🔍 Troubleshooting

### ❌ Las capas no se mueven:
- ✅ Verifica que `Auto Move` esté checked
- ✅ Verifica que `Base Speed > 0`
- ✅ Verifica que los `Layer Transform` estén asignados
- ✅ Asegúrate de que el juego esté en Play mode

### ❌ Se ve un corte/espacio entre loops:
- ✅ Verifica que `Sprite Width` sea correcto
- ✅ Asegúrate de que la copia esté exactamente a `X = Sprite Width`
- ✅ Los sprites deben tener los bordes que conecten perfectamente

### ❌ Una capa no se mueve:
- ✅ Verifica que `Parallax Speed > 0`
- ✅ Verifica que el `Layer Transform` esté asignado
- ✅ Revisa la consola por errores

### ❌ Las capas se mueven a la misma velocidad:
- ✅ Ajusta `Parallax Speed` de cada capa
- ✅ Ejemplo: Capa 0 = 0.5, Capa 1 = 1.5
- ✅ Mayor diferencia = más efecto de profundidad

### ❌ Las capas se mueven demasiado rápido:
- ✅ Reduce `Base Speed`
- ✅ O reduce `Parallax Speed` de cada capa

---

## 🎨 Tips para Mejores Resultados

### 1. **Número de Capas**
- ✅ 2 capas: Simple y efectivo
- ✅ 3 capas: Mayor profundidad (cielo + medio + suelo)
- ✅ 4+ capas: Muy cinematográfico

### 2. **Velocidades Recomendadas**
```
Capa 0 (Más lejana):  0.3 - 0.5
Capa 1 (Media):       0.8 - 1.0
Capa 2 (Cercana):     1.5 - 2.0
```

### 3. **Diseño de Sprites**
- ✅ Los sprites deben "conectar" en los bordes para loop sin cortes
- ✅ Usa colores/elementos que creen sensación de profundidad
- ✅ Las capas cercanas deben tener más detalle

### 4. **Optimización**
- ✅ Usa sprites optimizados (no muy grandes)
- ✅ Comprime las texturas en Unity
- ✅ No uses más de 4 capas para móvil

---

## 🚀 Funcionalidades del Script

### Métodos Públicos:

```csharp
// Pausar/reanudar movimiento
parallaxBackground.SetAutoMove(false); // Pausar
parallaxBackground.SetAutoMove(true);  // Reanudar

// Cambiar velocidad en runtime
parallaxBackground.SetBaseSpeed(3.5f);

// Resetear posiciones
parallaxBackground.ResetLayers();
```

### Ejemplo de Uso:
```csharp
// En tu DinoRunner, al pausar el juego:
ParallaxBackground parallax = FindFirstObjectByType<ParallaxBackground>();
if (parallax != null)
{
    parallax.SetAutoMove(false); // Detener parallax
}
```

---

## 📊 Estructura del Sistema

### Clase ParallaxLayer:
```csharp
[System.Serializable]
public class ParallaxLayer
{
    public Transform layerTransform;  // El sprite de fondo
    public float parallaxSpeed;       // Velocidad relativa
    public float spriteWidth;         // Ancho para loop
    public Vector3 startPosition;     // Posición inicial
}
```

### Lógica del Loop:
```
1. Capa se mueve a la izquierda
2. Si distancia movida >= spriteWidth:
   → Reposicionar a X + spriteWidth
   → Actualizar startPosition
3. Repetir infinitamente
```

---

## ✨ Mejoras Futuras Opcionales

- [ ] Agregar parallax vertical (para saltos)
- [ ] Sincronizar velocidad con velocidad del jugador
- [ ] Agregar efecto de fade entre capas
- [ ] Soportar múltiples copias por capa
- [ ] Agregar zoom dinámico
- [ ] Parallax basado en posición del mouse/jugador

---

## 📝 Checklist de Setup

```
□ Crear ParallaxManager GameObject
□ Agregar ParallaxBackground script
□ Crear Capa 0 (lejana) y su copia
□ Crear Capa 1 (cercana) y su copia
□ Asignar sprites a cada capa
□ Posicionar capas en Z correcto
□ Configurar Parallax Speed de cada capa
□ Medir y configurar Sprite Width
□ Configurar Base Speed
□ Activar Auto Move
□ Probar en Play mode
□ Ajustar velocidades según preferencia
```

---

**Fecha de Creación:** 19 de Octubre, 2025  
**Estado:** ✅ Completo y Funcional  
**Archivos:**
- ParallaxBackground.cs
- README_PARALLAX_BACKGROUND.md (este archivo)

**Resultado:**
- 🎨 Efecto de profundidad profesional
- 🔄 Loop infinito sin cortes
- ⚙️ Totalmente configurable
- 🎮 Fácil de usar
