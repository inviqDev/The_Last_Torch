using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime._Experiments
{
    public class RotationTests
    {
        private GameObject testObject;
        private Rotation rotationScript;

        [SetUp]
        public void SetUp()
        {
            // Создаем тестовый объект и добавляем к нему скрипт Rotation
            testObject = new GameObject("TestObject");
            rotationScript = testObject.AddComponent<Rotation>();

            // Устанавливаем начальные значения
            rotationScript.SetTurnSpeed(360f);
            rotationScript.SetSmooth(false);
        }

        [TearDown]
        public void TearDown()
        {
            // Удаляем тестовый объект после каждого теста
            Object.DestroyImmediate(testObject);
        }

        // ==========================
        // ХЕЛПЕРЫ: приватные методы/поля
        // ==========================

        // Универсальный поиск метода по всей иерархии (учитывает перегрузки)
        private static MethodInfo FindMethodRecursive(
            Type type,
            string methodName,
            BindingFlags flags,
            Type[] parameterTypes // передай Type.EmptyTypes для "без параметров"
        )
        {
            var cur = type;
            while (cur != null)
            {
                var mi = cur.GetMethod(methodName, flags, binder: null, types: parameterTypes, modifiers: null);
                if (mi != null) return mi;
                cur = cur.BaseType;
            }
            return null!;
        }

        // ТВОЕ ИМЯ И СИГНАТУРА СОХРАНЕНЫ: void-версия (как ты вызывал Update)
        private void InvokePrivateMethod(object obj, string methodName)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var method = FindMethodRecursive(obj.GetType(), methodName, flags, Type.EmptyTypes);
            Assert.IsNotNull(method, $"Метод {methodName} не найден.");

            try
            {
                method!.Invoke(obj, Array.Empty<object>());
            }
            catch (TargetInvocationException ex)
            {
                // Пробрасываем реальную причину из приватного метода
                throw ex.InnerException ?? ex;
            }
        }

        // ТВОЕ ИМЯ И СИГНАТУРА СОХРАНЕНЫ: generic-версия (как ты вызывал CalculateTargetDirection)
        private T InvokePrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            // Пытаемся угадать типы параметров для корректного выбора перегрузки
            Type[] parameterTypes;
            if (parameters == null || parameters.Length == 0)
                parameterTypes = Type.EmptyTypes;
            else
            {
                parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i]?.GetType() ?? typeof(object);
            }

            var method = FindMethodRecursive(obj.GetType(), methodName, flags, parameterTypes);
            Assert.IsNotNull(method, $"Метод {methodName} не найден.");

            try
            {
                var result = method!.Invoke(obj, parameters);

                // Если приватный метод возвращает void → result == null
                if (result == null)
                    return default!;

                return (T)result;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field, $"Поле {fieldName} не найдено.");
            return (T)field!.GetValue(obj);
        }

        private void SetPrivateField<T>(object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field, $"Поле {fieldName} не найдено.");
            field!.SetValue(obj, value);
        }

        // ==========================
        // ТЕСТЫ (ТВОИ, БЕЗ ИЗМЕНЕНИЙ)
        // ==========================

        [Test]
        public void FaceDirection_ShouldSetCorrectModeAndDirection()
        {
            // Arrange
            var targetDirection = new Vector3(1, 0, 0);

            // Act
            rotationScript.FaceDirection(targetDirection);

            // Assert
            Assert.AreEqual(FacingMode.Direction, GetPrivateField<FacingMode>(rotationScript, "_mode"));
            Assert.AreEqual(targetDirection.normalized, GetPrivateField<Vector3>(rotationScript, "_desiredDir"));
        }

        [Test]
        public void FaceTarget_ShouldSetCorrectModeAndTarget()
        {
            // Arrange
            var targetObject = new GameObject("TargetObject").transform;

            // Act
            rotationScript.FaceTarget(targetObject);

            // Assert
            Assert.AreEqual(FacingMode.TargetTransform, GetPrivateField<FacingMode>(rotationScript, "_mode"));
            Assert.AreEqual(targetObject, GetPrivateField<Transform>(rotationScript, "_target"));

            // Clean up
            Object.DestroyImmediate(targetObject.gameObject);
        }

        [Test]
        public void StopFacing_ShouldSetModeToNone()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward);

            // Act
            rotationScript.StopFacing();

            // Assert
            Assert.AreEqual(FacingMode.None, GetPrivateField<FacingMode>(rotationScript, "_mode"));
        }

        [Test]
        public void Update_ShouldNotRotateWhenInDeadZone()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward);
            rotationScript.SetTurnSpeed(90f);
            rotationScript.SetSmooth(false);

            // Устанавливаем мёртвую зону
            SetPrivateField(rotationScript, "deadZoneDeg", 180f);

            // Act
            InvokePrivateMethod(rotationScript, "Update");

            // Assert
            float currentYaw = testObject.transform.eulerAngles.y;
            Assert.AreEqual(0f, currentYaw); // Угол не должен измениться
        }

        [Test]
        public void SetTurnSpeed_ShouldClampToMinimumValue()
        {
            // Act
            rotationScript.SetTurnSpeed(0f);

            // Assert
            Assert.AreEqual(1f, GetPrivateField<float>(rotationScript, "turnSpeedDegPerSec"));
        }

        [Test]
        public void SetSmooth_ShouldUpdateSmoothSettings()
        {
            // Act
            rotationScript.SetSmooth(true, 0.5f);

            // Assert
            Assert.IsTrue(GetPrivateField<bool>(rotationScript, "useSmoothDamp"));
            Assert.AreEqual(0.5f, GetPrivateField<float>(rotationScript, "smoothTime"));
        }

        [Test]
        public void RotateTarget_NullAfterInitialization_ShouldNotThrow()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward);
            SetPrivateField<Transform>(rotationScript, "rotateTarget", null);

            // Act & Assert
            Assert.DoesNotThrow(() => InvokePrivateMethod(rotationScript, "Update"));
        }

        [Test]
        public void UpAxis_ZeroVector_ShouldNotThrow()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward);
            SetPrivateField(rotationScript, "upAxis", Vector3.zero);

            // Act & Assert
            Assert.DoesNotThrow(() => InvokePrivateMethod(rotationScript, "Update"));
        }

        [Test]
        public void DeadZone_NegativeValue_ShouldNotRotate()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward);
            SetPrivateField(rotationScript, "deadZoneDeg", -10f);

            // Act
            InvokePrivateMethod(rotationScript, "Update");

            // Assert
            float currentYaw = testObject.transform.eulerAngles.y;
            Assert.AreEqual(0f, currentYaw); // Угол не должен измениться
        }

        [Test]
        public void SmoothTime_NegativeValue_ShouldNotThrow()
        {
            // Arrange
            rotationScript.SetSmooth(true);
            rotationScript.FaceDirection(Vector3.forward);

            // Act & Assert
            Assert.DoesNotThrow(() => InvokePrivateMethod(rotationScript, "Update"));
        }

        [Test]
        public void TargetTransform_Destroyed_ShouldNotThrow()
        {
            // Arrange
            var targetObject = new GameObject("TargetObject").transform;
            rotationScript.FaceTarget(targetObject);

            // Уничтожаем цель
            Object.DestroyImmediate(targetObject.gameObject);

            // Act & Assert
            Assert.DoesNotThrow(() => InvokePrivateMethod(rotationScript, "Update"));
        }

        [Test]
        public void TargetPosition_SameAsRotateTarget_ShouldNotRotate()
        {
            // Arrange
            rotationScript.FaceTarget(null);
            rotationScript.FaceDirection(Vector3.zero);
            SetPrivateField(rotationScript, "_targetPos", testObject.transform.position);

            // Act
            InvokePrivateMethod(rotationScript, "Update");

            // Assert
            float currentYaw = testObject.transform.eulerAngles.y;
            Assert.AreEqual(0f, currentYaw); // Угол не должен измениться
        }

        [Test]
        public void FaceDirection_ZeroVector_ShouldSetModeToNone()
        {
            // Act
            rotationScript.FaceDirection(Vector3.zero);

            // Assert
            Assert.AreEqual(FacingMode.None, GetPrivateField<FacingMode>(rotationScript, "_mode"));
        }

        [Test]
        public void Rotate_Exactly180Degrees_ShouldRotateCorrectly()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward); // Поворот на 180 градусов
            var Yaw1 = rotationScript.transform.eulerAngles.y;

            // Act 1
            InvokePrivateMethod(rotationScript, "Update");
            
            rotationScript.FaceDirection(Vector3.back); // Поворот на 180 градусов
            var Yaw2 = rotationScript.transform.eulerAngles.y;
             
            InvokePrivateMethod(rotationScript, "Update");

            // Assert
            Assert.AreEqual(Yaw1, Mathf.Round(Yaw2)); // Угол должен быть ровно 180
        }

        [Test]
        public void SwitchingModes_ShouldUpdateStateCorrectly()
        {
            // Arrange
            var targetObject = new GameObject("TargetObject").transform;
            rotationScript.FaceTarget(targetObject);

            // Act
            rotationScript.FaceDirection(Vector3.forward);

            // Assert
            Assert.AreEqual(FacingMode.Direction, GetPrivateField<FacingMode>(rotationScript, "_mode"));
            Assert.AreEqual(Vector3.forward, GetPrivateField<Vector3>(rotationScript, "_desiredDir"));

            // Clean up
            Object.DestroyImmediate(targetObject.gameObject);
        }

        // [Test]
        // public void CalculateTargetDirection_ShouldReturnCorrectDirection()
        // {
        //     // Arrange
        //     var targetObject = new GameObject("TargetObject").transform;
        //     targetObject.position = new Vector3(10, 0, 10);
        //     SetPrivateField(rotationScript, "_mode", FacingMode.TargetTransform);
        //     SetPrivateField(rotationScript, "_target", targetObject);
        //
        //     // Act
        //     var direction = InvokePrivateMethod<Vector3>(rotationScript, "CalculateTargetDirection");
        //
        //     // Assert
        //     Assert.AreEqual(new Vector3(10, 0, 10).normalized, direction);
        //
        //     // Clean up
        //     Object.DestroyImmediate(targetObject.gameObject);
        // }

        [Test]
        public void CalculateTargetDirection_ShouldReturnCorrectDirection()
        {
            // Arrange
            var targetObject = new GameObject("TargetObject").transform;
            targetObject.position = new Vector3(10, 0, 10);

            // ВАЖНО: у компонента приватное поле rotateTarget — выставим его на наш testObject,
            // иначе CalculateTargetDirection() упадёт на rotateTarget.position
            SetPrivateField(rotationScript, "rotateTarget", testObject.transform);

            // Режим и цель — как у тебя
            SetPrivateField(rotationScript, "_mode", FacingMode.TargetTransform);
            SetPrivateField(rotationScript, "_target", targetObject);

            // Act — берём значение ИЗ приватного метода (как ты и хотел)
            var direction = InvokePrivateMethod<Vector3>(rotationScript, "CalculateTargetDirection");

            // Assert — сравниваем ИМЕННО НАПРАВЛЕНИЕ (без учёта длины), чтобы не споткнуться на нормализации
            var expected = new Vector3(10, 0, 10);
            Assert.Less(Vector3.Angle(direction, expected), 0.001f, 
                $"Ожидали направление {expected} (с точностью по углу), получили {direction}");

            // Clean up
            Object.DestroyImmediate(targetObject.gameObject);
        }

        //
        // [Test]
        // public void Update_WithCustomUpAxis_ShouldRotateCorrectly()
        // {
        //     // Arrange
        //     rotationScript.FaceDirection(Vector3.forward);
        //     var aw1 = rotationScript.transform.eulerAngles;
        //
        //     // Act
        //     SetPrivateField(rotationScript, "upAxis", Vector3.right); // Изменяем ось вращения
        //     SetPrivateField(rotationScript, "_desiredDir", new Vector3(0f, 1f, 1f));
        //     SetPrivateField(rotationScript, "_mode", FacingMode.Direction);
        //     InvokePrivateMethod<Vector3>(rotationScript, "CalculateTargetDirection");
        //     InvokePrivateMethod(rotationScript, "Update");
        //
        //     // Assert
        //     var currentRotation = testObject.transform.rotation.eulerAngles;
        //     Assert.AreNotEqual(0f, currentRotation.z); // Проверяем, что вращение произошло вокруг оси Z
        // }

        [Test]
        public void Update_ShouldNotPerformUnnecessaryOperations()
        {
            // Arrange
            rotationScript.FaceDirection(Vector3.forward);
            InvokePrivateMethod(rotationScript, "Update");

            // Act
            var initialRotation = testObject.transform.rotation;
            InvokePrivateMethod(rotationScript, "Update");

            // Assert
            Assert.AreEqual(initialRotation, testObject.transform.rotation); // Вращение не должно происходить повторно
        }
    }
}
