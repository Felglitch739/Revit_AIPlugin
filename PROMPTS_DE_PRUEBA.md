# 🎯 PROMPTS DE PRUEBA PARA REVIT AI PLUGIN

**Guía completa de prompts optimizados para probar cada funcionalidad del plugin.**

---

## 📋 ÍNDICE RÁPIDO

| Sección | Prompts |
|---------|---------|
| [Habitaciones Básicas](#habitaciones-básicas) | 4 ejemplos |
| [Muros Individuales](#muros-individuales) | 3 ejemplos |
| [Estructuras (Columnas/Vigas)](#estructuras-columnasvigas) | 4 ejemplos |
| [Puertas y Ventanas](#puertas-y-ventanas) | 3 ejemplos |
| [Muebles y Decoración](#muebles-y-decoración) | 4 ejemplos |
| [Proyectos Complejos](#proyectos-complejos) | 3 ejemplos |
| [Lectura de Elementos](#lectura-de-elementos) | 2 ejemplos |

---

## 🏠 HABITACIONES BÁSICAS

### 1️⃣ Habitación Simple - Dormitorio
```
Crea una habitación de 4 metros de ancho por 5 metros de largo
```

**Resultado esperado:**
- Una habitación rectangular de 4m x 5m
- 4 muros perimetrales
- Apta para agregar muebles

---

### 2️⃣ Habitación Mediana - Sala de Estar
```
Necesito una sala grande de 6 metros de ancho y 7 metros de largo
```

**Resultado esperado:**
- Habitación rectangular de 6m x 7m
- Espacio amplio para colocar sofá, mesa, etc.

---

### 3️⃣ Habitación Pequeña - Baño
```
Crea un baño compacto de 2 metros de ancho por 3 metros de largo
```

**Resultado esperado:**
- Habitación rectangular pequeña de 2m x 3m
- Apta para agregar muebles sanitarios

---

### 4️⃣ Habitación Rectangular Grande - Oficina
```
Haz una oficina espaciosa de 8 metros de ancho por 10 metros de largo
```

**Resultado esperado:**
- Habitación rectangular de 8m x 10m
- Espacio suficiente para múltiples escritorios

---

## 🧱 MUROS INDIVIDUALES

### 1️⃣ Muro Simple Horizontal
```
Crea un muro horizontal de 5 metros de largo en el origen
```

**Resultado esperado:**
- Un muro aislado de 5m de largo
- Posicionado en el nivel actual

---

### 2️⃣ Muro Vertical
```
Dibuja un muro vertical de 4 metros de altura
```

**Resultado esperado:**
- Un muro aislado de 4m de largo (en eje Y)

---

### 3️⃣ Muro en Ángulo
```
Crea un muro diagonal de 7 metros
```

**Resultado esperado:**
- Un muro aislado de 7m de largo
- Puede ser diagonal según el sistema

---

## 🏗️ ESTRUCTURAS (COLUMNAS/VIGAS)

### 1️⃣ Columna Aislada
```
Coloca una columna estructural en la posición X=2.5, Y=3
```

**Resultado esperado:**
- Una columna estructural de hormigón
- Posicionada en las coordenadas especificadas

---

### 2️⃣ Fila de Columnas
```
Crea 3 columnas estructurales en línea: primera en (1,2), segunda en (3,2), tercera en (5,2)
```

**Resultado esperado:**
- 3 columnas alineadas horizontalmente
- Espaciadas regularmente

---

### 3️⃣ Viga Simple
```
Dibuja una viga estructural desde el punto (1,1) hasta el punto (5,1)
```

**Resultado esperado:**
- Una viga horizontal conectando dos puntos
- De 4 metros de largo

---

### 4️⃣ Estructura Básica (Marco de Edificio)
```
Crea un marco estructural: 4 columnas en (1,1), (5,1), (1,5), (5,5) y vigas conectando los puntos
```

**Resultado esperado:**
- 4 columnas en las esquinas
- Vigas conectando los puntos
- Marco rectangular estructural de 4m x 4m

---

## 🚪 PUERTAS Y VENTANAS

### 1️⃣ Puerta en Pared Frontal
```
Crea una habitación de 4m x 5m y coloca una puerta simple en el centro de la pared frontal
```

**Resultado esperado:**
- Habitación rectangular de 4m x 5m
- Una puerta simple ("Single") en la posición central: X=2, Y=0

---

### 2️⃣ Ventana en Pared Lateral
```
Crea una sala de 6m x 8m, coloca una puerta en el centro frontal (Y=0) y una ventana fija en la pared derecha (X=6)
```

**Resultado esperado:**
- Habitación rectangular de 6m x 8m
- Puerta single en X=3, Y=0
- Ventana fija en X=6, Y=4 (centro de la pared derecha)

---

### 3️⃣ Múltiples Puertas y Ventanas
```
Diseña una habitación de 5m x 6m con: 
- Una puerta doble en el frente (Y=0, X=2.5)
- Dos ventanas operables en la pared izquierda (X=0, Y=1.5 y Y=4.5)
```

**Resultado esperado:**
- Habitación rectangular de 5m x 6m
- Puerta doble ("Double") en el centro frontal
- Dos ventanas operables ("Operable") en la pared izquierda

---

## 🛋️ MUEBLES Y DECORACIÓN

### 1️⃣ Dormitorio Amueblado
```
Crea un dormitorio de 4m x 5m y coloca:
- Una cama ("Bed") al fondo (centro): X=2, Y=3.8
- Una silla al lado del escritorio: X=1, Y=1.5
- Un escritorio en el rincón: X=1.2, Y=1.2
```

**Resultado esperado:**
- Habitación rectangular de 4m x 5m
- Cama al fondo contra la pared
- Escritorio con silla en la esquina

---

### 2️⃣ Sala de Estar Completa
```
Diseña una sala de 6m x 7m con:
- Un sofá ("Sofa") en el centro: X=3, Y=2
- Una mesa de centro ("Table"): X=3, Y=3.5
- Dos sillas de descanso ("Seating"): X=1, Y=2 y X=5, Y=2
```

**Resultado esperado:**
- Sala amplia de 6m x 7m
- Sofá en la zona central
- Mesa de centro frente al sofá
- Dos sillas laterales

---

### 3️⃣ Oficina Profesional
```
Crea una oficina de 5m x 6m con:
- Tres escritorios ("Desk"): 
  - Escritorio 1: X=1.5, Y=1.5
  - Escritorio 2: X=1.5, Y=3
  - Escritorio 3: X=1.5, Y=4.5
- Tres sillas de trabajo ("Chair"): asociadas a cada escritorio
```

**Resultado esperado:**
- Oficina de 5m x 6m
- 3 escritorios en fila
- 3 sillas para trabajar

---

### 4️⃣ Cocina Moderna
```
Haz una cocina de 3.5m x 4m y coloca:
- Una mesa de comedor ("Dining"): X=1.75, Y=2
- Cuatro sillas de comedor ("Seating"): alrededor de la mesa
- Almacenamiento ("Storage"): X=0.8, Y=0.8
```

**Resultado esperado:**
- Cocina compacta de 3.5m x 4m
- Zona de comida con mesa y sillas
- Zona de almacenamiento

---

## 🏢 PROYECTOS COMPLEJOS

### 1️⃣ Vivienda Studio Completa
```
Diseña un studio de 5m x 6m con:
- Un muro divisor a mitad de la habitación en X=2.5
- Una cama ("Bed") en la zona privada: X=1.25, Y=3
- Un sofá ("Sofa") en la zona común: X=3.75, Y=2
- Una mesa ("Dining"): X=3.75, Y=3.5
- Una puerta simple en el frente: X=2.5, Y=0
- Una ventana fija en la pared derecha: X=5, Y=3
```

**Resultado esperado:**
- Studio dividido con muros, cama, sofá, mesa
- Entrada por la puerta delantera
- Iluminación natural por ventana

---

### 2️⃣ Galería Comercial
```
Crea una galería de 10m x 12m con:
- 2 columnas estructurales: (2,2) y (8,2)
- 2 columnas más: (2,10) y (8,10)
- Una vigas conectando: (2,2)→(8,2), (2,10)→(8,10), (2,2)→(2,10), (8,2)→(8,10)
- Dos puertas deslizantes ("Sliding") en el frente: X=3, Y=0 y X=7, Y=0
```

**Resultado esperado:**
- Galería grande con estructura modular
- 4 columnas y 4 vigas formando un marco
- 2 accesos deslizantes

---

### 3️⃣ Casa Pequeña (2 Habitaciones)
```
Diseña una casa de 2 plantas:
Planta baja - Sala de 6m x 8m:
- Una puerta simple en Y=0, X=3
- Dos ventanas fijas en la pared derecha (X=6): Y=2 y Y=6
- Un sofá ("Sofa"): X=3, Y=2
- Una mesa ("Table"): X=3, Y=4
- Una silla ("Seating"): X=1, Y=2

Nota: Repetir después para crear la habitación de arriba
```

**Resultado esperado:**
- Sala de estar amueblada de 6m x 8m
- Acceso por puerta frontal
- Iluminación por ventanas laterales
- (Luego crear habitación similar)

---

## 📖 LECTURA DE ELEMENTOS

### 1️⃣ Contar Muros Existentes
```
¿Cuántos muros hay en el modelo actualmente? Lee los elementos de categoría de muros
```

**Resultado esperado:**
- Listado de muros existentes
- Cantidad total de muros
- Propiedades básicas

---

### 2️⃣ Inspeccionar Elementos Estructurales
```
Lee todos los elementos estructurales en el modelo: columnas, vigas y muros
```

**Resultado esperado:**
- Listado de columnas (si existen)
- Listado de vigas (si existen)
- Listado de muros estructurales

---

## 🎬 SECUENCIA RECOMENDADA DE PRUEBAS

### **Fase 1: Validación Básica** (5-10 minutos)
1. ✅ Habitación Simple (4m x 5m)
2. ✅ Lectura de elementos (contar muros)
3. ✅ Puerta en pared frontal

### **Fase 2: Muebles y Detalles** (10-15 minutos)
4. ✅ Dormitorio amueblado
5. ✅ Sala de estar completa
6. ✅ Múltiples puertas y ventanas

### **Fase 3: Estructuras** (10-15 minutos)
7. ✅ Columna aislada
8. ✅ Fila de columnas
9. ✅ Viga simple
10. ✅ Marco estructural básico

### **Fase 4: Proyectos Complejos** (15-20 minutos)
11. ✅ Studio completo
12. ✅ Galería comercial
13. ✅ Casa pequeña (2 habitaciones)

---

## 💡 TIPS DE USO

### ✅ Prompts que FUNCIONAN BIEN:
- Especificar dimensiones exactas: `"4 metros de ancho por 5 de largo"`
- Usar nombres de categoría: `"sofá"`, `"cama"`, `"escritorio"`
- Especificar posiciones claras: `"en el centro"`, `"en la esquina"`, `"al fondo"`
- Usar el sistema de pares (X, Y): `"en la posición (2, 3)"`

### ❌ Prompts que pueden CAUSAR PROBLEMAS:
- Dimensiones vagas: `"una habitación normal"` → La IA inventará tamaños
- Posiciones imprecisas: `"pon un sofá por ahí"` → Riesgo de colisiones
- Referencias a elementos no disponibles: `"una silla de diseño italiana"` → La IA usará la más cercana

### 🔧 SOLUCIONES SI FALLA UN PROMPT:
1. **Reintenta con dimensiones explícitas** (números exactos)
2. **Especifica coordenadas con (X, Y)** en lugar de descripciones vagas
3. **Usa solo tipos de muebles estándar** de Revit (Desk, Chair, Bed, Table, Sofa, Storage)
4. **Para puertas/ventanas**, recuerda que DEBEN estar en el perímetro (Y=0, Y=largo, X=0, X=ancho)

---

## 📊 PARÁMETROS DE REFERENCIA

### Tipos de Muebles Disponibles:
- **Desk** → Escritorios
- **Chair** / **Seating** → Sillas
- **Bed** / **Double** / **Twin** → Camas
- **Table** / **Dining** → Mesas
- **Sofa** / **Couch** → Sofás
- **Storage** / **Cabinet** / **Shelving** → Almacenamiento

### Tipos de Puertas:
- **Single** → Puerta simple (1 hoja)
- **Double** → Puerta doble (2 hojas)
- **Sliding** → Puerta corredera
- **Bifold** → Puerta plegable

### Tipos de Ventanas:
- **Fixed** → Ventana fija
- **Operable** → Ventana corredera/abatible
- **Glass** → Panel de vidrio
- **Double-Hung** → Ventana de guillotina

### Restricciones de Seguridad (Padding):
- Muebles: Mínimo 0.6m desde cualquier muro
- Rango X válido: `0.6 a (ancho - 0.6)`
- Rango Y válido: `0.6 a (largo - 0.6)`
- Separación entre muebles: Mínimo 1.5m

---

## 🚀 VERSIÓN COMPLETA: PROYECTO INTEGRAL

```
Diseña una casa de 15m x 18m con:

ESPACIOS:
1. Sala de 6m x 7m (entrada izquierda frontal)
   - Puerta simple en (3, 0)
   - Dos ventanas fijas en pared derecha: (6, 2) y (6, 5)
   - Sofá en (3, 2), Mesa en (3, 3.5), Dos sillas en (1, 2) y (5, 2)

2. Cocina de 4m x 5m (contigua a sala)
   - Puerta doble hacia sala en (4, 7)
   - Mesa de comedor en (2, 8), Cuatro sillas alrededor
   - Almacenamiento en (0.8, 7.8)

3. Dormitorio de 5m x 6m (lado derecho)
   - Puerta simple hacia pasillo en (8, 6)
   - Cama doble en (6.5, 11), Escritorio en (8.5, 7), Silla en (8.5, 7.5)

4. Estructura:
   - 2 columnas de soporte: (6, 3) y (6, 12)
   - Viga conectando: (6, 3) a (6, 12)
```

**Resultado esperado:**
- Casa funcional de 15m x 18m
- 3 espacios diferenciados
- Estructura de soporte
- Iluminación y acceso adecuados

---

**¡Prueba estos prompts y reporta cualquier inconsistencia o mejora necesaria!** 🎯

