# 🚀 COMANDOS DE PRUEBA RÁPIDA

Aquí tienes ejemplos listos para copiar/pegar en el chat del plugin para probar las nuevas funciones.

---

## ✅ Prueba 1: Crear Columna Simple

```
Crea una columna estructural en la posición X=2 metros, Y=2 metros
```

**Resultado esperado:**
```
✅ Columna creada con éxito.
• Tipo: [tipo detectado]
• Nivel: Level 1
• Posición: X=2.0m, Y=2.0m
• Altura: 3.0m
```

---

## ✅ Prueba 2: Crear Viga

```
Crea una viga estructural desde el punto (0, 0) hasta (5, 0)
```

**Resultado esperado:**
```
✅ Viga creada con éxito.
• Tipo: [tipo detectado]
• Nivel: Level 1
• Desde: X=0.0m, Y=0.0m
• Hasta: X=5.0m, Y=0.0m
• Longitud: 5.00m
```

---

## ✅ Prueba 3: Crear Techo

```
Crea un techo en el nivel Level 1
```

**Resultado esperado:**
```
✅ Preparados 1 techo(s) para crear.
• Tipo: [tipo detectado]
• Nivel: Level 1
• Nota: Puedes crear techos combinando los muros disponibles.
```

**Nota:** Debe haber muros en Level 1 para que funcione.

---

## ✅ Prueba 4: Habitación Completa con Estructura

```
Crea una habitación de 6 metros de ancho por 5 de largo, luego agrega 4 columnas 
en las esquinas interiores (1.5, 1.5), (4.5, 1.5), (1.5, 3.5), (4.5, 3.5), 
crea un techo y coloca una puerta en el centro
```

**Lo que debería suceder:**
1. Se crea habitación rectangular (muros perimetrales)
2. Se crean 4 columnas en posiciones especificadas
3. Se crea techo
4. Se coloca puerta

---

## ✅ Prueba 5: Grid de Columnas (Estructura Industrial)

```
Crea una habitación de 12m x 10m, luego agrega un grid de columnas 
separadas cada 3 metros en ambas direcciones
```

**Patrón generado:**
```
(1.5, 2.5), (4.5, 2.5), (7.5, 2.5), (10.5, 2.5)
(1.5, 5.0), (4.5, 5.0), (7.5, 5.0), (10.5, 5.0)
(1.5, 7.5), (4.5, 7.5), (7.5, 7.5), (10.5, 7.5)
```

---

## ✅ Prueba 6: Vigas de Soporte

```
Crea una habitación de 8m x 6m, luego agrega 2 vigas longitudinales 
en Y=2 y Y=4, que corran de X=0 a X=8
```

---

## ✅ Prueba 7: Arquitectura Residencial Simulada

```
Diseña una casa modular:
- Habitación principal: 5m x 4m
- Baño: 2m x 2.5m  
- Cocina: 3m x 3.5m
- Sala: 6m x 5m
Con columnas de soporte, techos y accesos
```

---

## ✅ Prueba 8: Combinar Todas las Herramientas

```
Crea un espacio arquitectónico completo que incluya:
- Una habitación estructurada de 6m x 5m
- Columnas interiores de soporte
- Una viga de arriostramiento
- Un techo
- Puertas de acceso
- Ventanas para iluminación
- Mobiliario interior (cama, escritorio, silla)
```

---

## 🧪 Pruebas de Edge Cases

### Prueba: Sin familias cargadas
```
Crea una columna de tipo "FamiliaInexistente" en X=2, Y=2
```
**Esperado:** Usar fallback automático a primera familia disponible

### Prueba: Nivel inexistente
```
Crea una viga en el nivel "Level 99" desde (0,0) a (5,0)
```
**Esperado:** Error descriptivo: "No se encontraron niveles en el modelo."

### Prueba: Techo sin muros
```
Crea un techo en Level 1 (sin haber creado muros antes)
```
**Esperado:** Error: "No hay muros en este nivel para crear techo."

---

## 📊 Matriz de Prueba Completa

| # | Función | Comando | Status |
|----|----------|---------|--------|
| 1 | CrearColumna | "Crea columna en X=2, Y=2" | ✅ |
| 2 | CrearViga | "Viga de (0,0) a (5,0)" | ✅ |
| 3 | CrearTecho | "Techo en Level 1" | ✅ |
| 4 | CrearHabitacion | "Habitación 6x5x3" | ✅ |
| 5 | ColocarPuerta | "Puerta en X=3, Y=0" | ✅ |
| 6 | ColocarVentana | "Ventana en X=1.5, Y=0" | ✅ |
| 7 | ColocarMobiliario | "Cama en X=3, Y=4" | ✅ |
| 8 | LeerElementos | "Lee muros en el modelo" | ✅ |
| 9 | Combinadas | Proyecto arquitectónico completo | ✅ |

---

## 🔄 Flujo de Prueba Recomendado

### Sesión 1: Pruebas Individuales
```
1. Prueba 1: CrearColumna
2. Prueba 2: CrearViga
3. Prueba 3: CrearTecho
```
**Duración:** 5 minutos

### Sesión 2: Pruebas Integradas
```
1. Prueba 4: Habitación + Columnas
2. Prueba 5: Grid de Columnas
3. Prueba 6: Vigas de Soporte
```
**Duración:** 10 minutos

### Sesión 3: Prueba Completa
```
1. Prueba 7: Arquitectura Residencial
2. Prueba 8: Espacio Arquitectónico Completo
3. Edge Cases (Pruebas de robustez)
```
**Duración:** 15 minutos

---

## 💡 Tips de Uso

### Tip 1: Usa números redondos
```
✅ BUENO: X=2, Y=3 (números simples)
❌ MALO: X=2.37456, Y=3.14159 (muy preciso)
```

### Tip 2: Especifica nivel si vas a agregar más niveles
```
✅ CrearColumna con "nivel: 'Level 2'" si planeas usar múltiples niveles
```

### Tip 3: Nombre tipos de familia si conoces cuáles están cargadas
```
✅ "Crea una viga de tipo 'IPE 300'" si sabes que existe
❌ "Crea cualquier viga" (fallback puede no ser lo esperado)
```

### Tip 4: Separa columnas al menos 1.5m entre sí
```
✅ Columnas en (1, 1), (4, 1) - separadas 3m
❌ Columnas en (1, 1), (1.2, 1) - muy cercanas
```

---

## 🐛 Troubleshooting Rápido

### "No se encontraron niveles en el modelo"
**Solución:** Crea al menos un muro o habitación primero

### "No se encontraron familias de columnas"
**Solución:** Carga una familia de columna en Revit (File > Load Family)

### "No se encontraron tipos de techo"
**Solución:** Carga tipo de techo en Revit (similar a columnas)

### Columna/Viga fuera de lugar
**Solución:** Verifica que:
- X, Y están en metros (no pies)
- Nivel es correcto ("Level 1", "Level 2", etc.)
- Coordenadas están dentro del rango lógico

---

## 📚 Documentación Completa

Para información detallada, consulta:
- `IMPLEMENTATION_SUMMARY.md` - Plan completo
- `USAGE_GUIDE_NEW_FEATURES.md` - Guía exhaustiva
- `TECHNICAL_SUMMARY.md` - Detalles técnicos
- `README_FINAL.md` - Resumen ejecutivo

---

## ✨ ¡Listo para empezar!

Copia cualquiera de los comandos anteriores y pégalo en el chat del plugin. 

**La IA automáticamente:**
1. Reconocerá qué función usar
2. Extraerá los parámetros
3. Ejecutará el handler en Revit
4. Te dará un resultado

¡Que disfrutes usando el plugin! 🎉
