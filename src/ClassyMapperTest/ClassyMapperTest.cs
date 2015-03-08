#region << Usings >>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClassyMapper;
using ClassyMapper.Attributes;
using ClassyMapper.Enums;
using ClassyMapper.Exceptions;
using ClassyMapper.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace ClassyTest
{
    [TestClass]
    public class ClassyMapperTest
    {

        #region << Basic Mapping Tests >>

        [TestMethod]
        public void MapEntityToDto_AllProperties()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<TestDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_AllProperties()
        {
            // Arrange
            TestDto input = new TestDto { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<TestEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void MapEntityToDto_AllProperties_ExpressionWay()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New(new ClassyMapperConfig {ExpressionTreeGetSetCalls = true})
                .Map<TestDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        #endregion

        #region << Enum Mapping Tests >>

        [TestMethod]
        public void MapEntityToDtoWithEnum_AllProperties()
        {
            // Arrange
            EnumEntity input = new EnumEntity { MyTest = 1, TesterId = 9304, Check = 1 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<EnumDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(TestEnum.MyValue, result.MyTest, "Property 'MyTest' does not match.");
            Assert.AreEqual(TestEnum.MyValue, result.Check, "Property 'Check' does not match.");
        }

        [TestMethod]
        public void MapEntityToDtoWithEnum_AllProperties_NewingUpDirectly()
        {
            // Arrange
            EnumEntity input = new EnumEntity { MyTest = 1, TesterId = 9304 , Check = 2};

            // Act
            var result = new ClassyMapper.ClassyMapper().Map<EnumDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(TestEnum.MyValue, result.MyTest, "Property 'MyTest' does not match.");
            Assert.AreEqual(TestEnum.YourValue, result.Check, "Property 'Check' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntityWithEnum_AllProperties()
        {
            // Arrange
            EnumDto input = new EnumDto { MyTest = TestEnum.MyValue, TesterId = 9304, Check = TestEnum.YourValue };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<EnumEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(1, result.MyTest, "Property 'MyTest' does not match.");
            Assert.AreEqual(2, result.Check, "Property 'Check' does not match.");
        }

        [TestMethod]
        public void MapEntityToDtoWithEnum_AllProperties_EnumDoesNotExst()
        {
            // Arrange
            EnumEntity input = new EnumEntity { MyTest = 12, TesterId = 9304 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<EnumDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(TestEnum.Unknown, result.MyTest, "Property 'MyTest' does not match.");
        }

        [TestMethod]
        public void MapEntityToDtoWithEnum_AllProperties_StringToEnum()
        {
            // Arrange
            EnumEntity2 input = new EnumEntity2 { MyTest = "MyValue", TesterId = 9304, Check = "MyValue" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<EnumDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(TestEnum.MyValue, result.MyTest, "Property 'MyTest' does not match.");
            Assert.AreEqual(TestEnum.MyValue, result.Check, "Property 'Check' does not match.");
        }

        [TestMethod]
        public void MapEntityToDtoWithEnum_AllProperties_EnumToString()
        {
            // Arrange
            EnumDto input = new EnumDto { MyTest = TestEnum.MyValue, TesterId = 9304, Check = TestEnum.YourValue };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<EnumEntity2>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual("MyValue", result.MyTest, "Property 'MyTest' does not match.");
            Assert.AreEqual("YourValue", result.Check, "Property 'Check' does not match.");
        }

        [TestMethod]
        public void MapEntityToDtoWithEnum_AllProperties_ReuseInstance()
        {
            // Arrange
            EnumEntity input = new EnumEntity { MyTest = 1, TesterId = 9304 };
            var mapper = ClassyMapper.ClassyMapper.New();

            // Act
            mapper.Map<EnumDto>(input);
            input.MyTest = 2;
            input.TesterId = 3;
            var result = mapper.Map<EnumDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.TesterId, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(TestEnum.YourValue, result.MyTest, "Property 'MyTest' does not match.");
        }

        #endregion

        #region << CustomMap Tests >>

        [TestMethod]
        public void MapEntityToDto_AllProperties_WithCustomMap()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().RegisterCustomMap<TestEntity, TestDto>(
                (from, to) =>
                {
                    to.A = from.B;
                }).Map<TestDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.B, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }


        [TestMethod]
        public void MapDtoToEntity_AllProperties_WithCustomMap()
        {
            // Arrange
            TestDto input = new TestDto { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().RegisterCustomMap<TestDto, TestEntity>(
                (from, to) =>
                {
                    to.A = from.B;
                }).Map<TestEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.B, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        #endregion

        #region << FromObjects Tests >>

        [TestMethod]
        public void MapClass_AllProperties_WithFromObjects()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };
            TestEntity2 input2 = new TestEntity2 { C = "3" };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New()
                    .RegisterFromObjects<TestEntity>(from => new object[] { from, input2 })
                    .Map<TestDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input2.C, result.C, "Property 'C' does not match.");
        }

        [TestMethod]
        public void MapList_AllProperties_WithFromObjects()
        {
            // Arrange
            List<TestEntity> input = new List<TestEntity>
            {
                new TestEntity {A = "1", B = "2"},
                new TestEntity {A = "4", B = "5"},
            };
            TestEntity2 input2 = new TestEntity2 { C = "3" };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New()
                    .RegisterFromObjects<TestEntity>(from => new object[] { from, input2 })
                    .MapToList<TestDto, TestEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input[0].A, result[0].A, "[0] Property 'A' does not match.");
            Assert.AreEqual(input[0].B, result[0].B, "[0] Property 'B' does not match.");
            Assert.AreEqual(input[1].A, result[1].A, "[1] Property 'A' does not match.");
            Assert.AreEqual(input[1].B, result[1].B, "[1] Property 'B' does not match.");
            Assert.AreEqual(input2.C, result[0].C, "[0] Property 'C' does not match.");
            Assert.AreEqual(input2.C, result[1].C, "[0] Property 'C' does not match.");
        }

        #endregion

        #region << Inner Class Tests >>

        [TestMethod]
        public void MapList_AllPropertiesWithInnerClass_WithFromObjects()
        {
            // Arrange
            List<TestEntity> input = new List<TestEntity>
            {
                new TestEntity {A = "1", B = "2", Inner = new TestEntityInner {D = "10"}},
                new TestEntity {A = "4", B = "5", Inner = new TestEntityInner {D = "11"}},
            };
            TestEntity2 input2 = new TestEntity2 { C = "3" };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New()
                    .RegisterFromObjects<TestEntity>(from => new object[] { from, input2, from.Inner })
                    .MapToList<TestDto, TestEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input[0].A, result[0].A, "[0] Property 'A' does not match.");
            Assert.AreEqual(input[0].B, result[0].B, "[0] Property 'B' does not match.");
            Assert.AreEqual(input[0].Inner.D, result[0].F, "[0] Property 'F' does not match.");
            Assert.AreEqual(input[1].A, result[1].A, "[1] Property 'A' does not match.");
            Assert.AreEqual(input[1].B, result[1].B, "[1] Property 'B' does not match.");
            Assert.AreEqual(input[1].Inner.D, result[1].F, "[1] Property 'F' does not match.");

            Assert.AreEqual(input2.C, result[0].C, "[0] Property 'C' does not match.");
            Assert.AreEqual(input2.C, result[1].C, "[0] Property 'C' does not match.");
        }

        #endregion

        #region << Complex Object Tests >>

        [TestMethod]
        public void MapClass_Complex_AllProperties()
        {
            // Arrange
            ComplexEntity input = new ComplexEntity { A = 1, B = 2, Inner = new ComplexEntityInner { B = "3" } };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ComplexDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B.ToString(CultureInfo.InvariantCulture), result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.Inner.B, result.Inner.B, "Property 'Inner.B' does not match.");
        }

        [TestMethod]
        public void MapClass_Complex_AllPropertiesExceptInner()
        {
            // Arrange
            ComplexEntity input = new ComplexEntity { A = 1, B = 2 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ComplexDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B.ToString(CultureInfo.InvariantCulture), result.B, "Property 'B' does not match.");
            Assert.AreEqual(null, result.Inner, "Property 'Inner' does not match.");
        }

        [TestMethod]
        public void MapClass_Complex_AllPropertiesExceptInner_WithIsNull()
        {
            // Arrange
            ComplexEntity input = new ComplexEntity { A = 1, B = 2 };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New(new ClassyMapperConfig {CreateToObjectFromNullFromObject = true})
                    .Map<ComplexDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B.ToString(CultureInfo.InvariantCulture), result.B, "Property 'B' does not match.");
            Assert.AreEqual(true, result.Inner.IsNull, "Property 'Inner' does not match.");
        }

        #endregion

        #region << Recursion Tests >>

        [TestMethod]
        public void MapClass_InfiniteRecursion_NullParent_AllProperties()
        {
            // Arrange
            RecursionParentEntity input = new RecursionParentEntity
            {
                A = "1",
                Child = new RecursionChildEntity { B = "2" }
            };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New(new ClassyMapperConfig {CreateToObjectFromNullFromObject = true})
                    .Map<RecursionParentDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.Child.B, result.Child.B, "Property 'Child.B' does not match.");
            Assert.AreEqual(true, result.Child.Parent.IsNull, "Property 'Child.Parent.IsNull' is false.");
            Assert.AreEqual(true, result.Child.Parent.Child.IsNull, "Property 'Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.IsNull,
                "PProperty 'Child.Parent.Child.Parent.Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.' is false.");
            Assert.AreEqual(
                null,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child.Parent.Child' is not false.");
        }

        [TestMethod]
        public void MapClass_InfiniteRecursion_NullParent_AllProperties_5MaxDeep()
        {
            // Arrange
            RecursionParentEntity input = new RecursionParentEntity
            {
                A = "1",
                Child = new RecursionChildEntity { B = "2" }
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { CreateToObjectFromNullFromObject = true, MaxNullDepth = 5 }).Map<RecursionParentDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.Child.B, result.Child.B, "Property 'Child.B' does not match.");
            Assert.AreEqual(true, result.Child.Parent.IsNull, "Property 'Child.Parent.IsNull' is false.");
            Assert.AreEqual(true, result.Child.Parent.Child.IsNull, "Property 'Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.IsNull' is false.");
            Assert.AreEqual(
                true,
                result.Child.Parent.Child.Parent.Child.Parent.Child.IsNull,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.IsNull' is false.");
            Assert.AreEqual(
                null,
                result.Child.Parent.Child.Parent.Child.Parent.Child.Parent,
                "Property 'Child.Parent.Child.Parent.Child.Parent.Child.Parent' is not null.");
        }

        #endregion

        #region << Nullable Tests >>

        [TestMethod]
        public void MapEntityToDto_AllNullable_AllProperties()
        {
            // Arrange
            NullableEntity input = new NullableEntity { C = 1, D = DateTime.UtcNow };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<NullableDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.A, "Property 'A' does not match.");
            Assert.AreEqual(DateTime.MinValue, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
            Assert.AreEqual(input.D, result.D, "Property 'D' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_AllNullable_AllProperties()
        {
            // Arrange
            NullableDto input = new NullableDto { A = 10, B = DateTime.UtcNow };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<NullableEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(0, result.C, "Property 'C' does not match.");
            Assert.AreEqual(DateTime.MinValue, result.D, "Property 'D' does not match.");
        }

        [TestMethod]
        public void MapEntityToDto_AllNullableButAssigned_AllProperties()
        {
            // Arrange
            NullableEntity input = new NullableEntity { A = 10, B = DateTime.Now, C = 1, D = DateTime.UtcNow };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<NullableDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
            Assert.AreEqual(input.D, result.D, "Property 'D' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_NullableButAssigned_AllProperties()
        {
            // Arrange
            NullableDto input = new NullableDto { A = 10, B = DateTime.UtcNow, C = 20, D = DateTime.Now };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<NullableEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
            Assert.AreEqual(input.D, result.D, "Property 'D' does not match.");
        }

        #endregion

        #region << UpCast/DownCast Tests >>

        [TestMethod]
        public void MapEntityToDto_UpCast_AllProperties()
        {
            // Arrange
            UpCastEntity input = new UpCastEntity { Test = 5 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<UpCastDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Test, "Property 'Test' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_DownCast_AllProperties()
        {
            // Arrange
            UpCastDto input = new UpCastDto { Test = 5 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<UpCastEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Test, "Property 'Test' does not match.");
        }

        #endregion

        #region << Namespace Tests >>

        [TestMethod]
        public void MapEntityToDto_NamespaceMapsCorrectly_AllProperties()
        {
            // Arrange
            NamespaceEntity input = new NamespaceEntity { A = "1" };
            NamespaceEntity2 input2 = new NamespaceEntity2 { A = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New()
                .RegisterFromObjects<NamespaceEntity>(from => new object[] { from, input2 })
                .Map<NamespaceDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input2.A, result.AA, "Property 'AA' does not match.");
        }

        #endregion

        #region << Timestamp Tests >>

        [TestMethod]
        public void MapEntityToDto_Timestamp_AllProperties()
        {
            // Arrange
            TimestampEntity input = new TimestampEntity { Timestamp = new byte[] { 1, 2, 3, 4, 5} };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<TimestampDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Convert.ToBase64String(input.Timestamp), result.Timestamp, "Property 'Timestamp' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_Timestamp_AllProperties()
        {
            // Arrange
            TimestampDto input = new TimestampDto { Timestamp = "AQIDBAU=" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<TimestampEntity>(input);

            // Assert
            byte[] check = Convert.FromBase64String(input.Timestamp);

            Assert.IsNotNull(result);
            Assert.AreEqual(check[0], result.Timestamp[0], "Property 'Timestamp' does not match.");
            Assert.AreEqual(check[1], result.Timestamp[1], "Property 'Timestamp' does not match.");
            Assert.AreEqual(check[2], result.Timestamp[2], "Property 'Timestamp' does not match.");
            Assert.AreEqual(check[3], result.Timestamp[3], "Property 'Timestamp' does not match.");
            Assert.AreEqual(check[4], result.Timestamp[4], "Property 'Timestamp' does not match.");
        }

        #endregion

        #region << SubList Tests >>

        [TestMethod]
        public void MapEntityToDto_SubList_NoListMap_AllProperties()
        {
            // Arrange
            SubListEntity input = new SubListEntity
            {
                A = "1",
                List = new List<SubListEntity2>
                {
                    new SubListEntity2 { A = "1"},
                    new SubListEntity2 { A = "2"}
                }
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { IgnoreLists = true }).Map<SubListDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.IsNull(result.List);
        }

        [TestMethod]
        public void MapEntityToDto_SubList_AllProperties()
        {
            // Arrange
            SubListEntity input = new SubListEntity
            {
                A = "1",
                List = new List<SubListEntity2>
                {
                    new SubListEntity2 { A = "1"},
                    new SubListEntity2 { A = "2"}
                }
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<SubListDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.List.Count, result.List.Count, "Property 'List' does not match.");
            Assert.AreEqual(input.List[0].A, result.List[0].A, "Property 'List[0].A' does not match.");
            Assert.AreEqual(input.List[1].A, result.List[1].A, "Property 'List[1].A' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_SubList_AllProperties()
        {
            // Arrange
            SubListDto input = new SubListDto
            {
                A = "1",
                List = new List<SubListDto2>
                {
                    new SubListDto2 { A = "1"},
                    new SubListDto2 { A = "2"}
                }
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<SubListEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.List.Count, result.List.Count, "Property 'List' does not match.");
            Assert.AreEqual(input.List[0].A, result.List[0].A, "Property 'List[0].A' does not match.");
            Assert.AreEqual(input.List[1].A, result.List[1].A, "Property 'List[1].A' does not match.");
        }

        [TestMethod]
        public void MapEntityToDto_SubList_NullList()
        {
            // Arrange
            SubListEntity input = new SubListEntity
            {
                A = "1",
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<SubListDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(0, result.List.Count, "Property 'List' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_SubList_NullList()
        {
            // Arrange
            SubListDto input = new SubListDto
            {
                A = "1",
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<SubListEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(0, result.List.Count, "Property 'List' does not match.");
        }

        [TestMethod]
        public void MapEntityToDto_SubList_NullList_SkipMap()
        {
            // Arrange
            SubListEntity input = new SubListEntity
            {
                A = "1",
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { MapEmptyListFromNullList = false }).Map<SubListDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(null, result.List, "Property 'List' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_SubList_NullList_SkipMap()
        {
            // Arrange
            SubListDto input = new SubListDto
            {
                A = "1",
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { MapEmptyListFromNullList = false }).Map<SubListEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(null, result.List, "Property 'List' does not match.");
        }

        #endregion

        #region << CopyValues Tests >>

        [TestMethod]
        public void CopyValues_SameClass()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };
            TestEntity result = new TestEntity();

            // Act
            ClassyMapper.ClassyMapper.CopyValues(input, result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void CopyValues_DifferentClass_SameProperties()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };
            TestDto result = new TestDto();

            // Act
            ClassyMapper.ClassyMapper.CopyValues(input, result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void CopyValues_DifferentClass_DifferentProperties()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };
            EnumDto result = new EnumDto();

            // Act
            ClassyMapper.ClassyMapper.CopyValues(input, result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.TesterId, "Property 'TesterId' does not match.");
            Assert.AreEqual(TestEnum.Unknown, result.MyTest, "Property 'MyTest' does not match.");
        }

        #endregion

        #region << Default String Methods >>

        [TestMethod]
        public void DefaultStringValues_NullProperties()
        {
            // Arrange
            TestEntity result = new TestEntity();

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValues(result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.A, "Property 'A' does not match.");
            Assert.AreEqual(string.Empty, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void DefaultStringValues_AllProperties()
        {
            // Arrange
            TestEntity result = new TestEntity { A = "G", B = "H" };

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValues(result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.A, "Property 'A' does not match.");
            Assert.AreEqual(string.Empty, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void DefaultStringValues_NullProperties_DefaultA()
        {
            // Arrange
            TestEntity result = new TestEntity();
            string defaultString = "A";

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValues(result, defaultString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(defaultString, result.A, "Property 'A' does not match.");
            Assert.AreEqual(defaultString, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void DefaultStringValues_AllProperties_DefaultA()
        {
            // Arrange
            TestEntity result = new TestEntity {A = "B", B = "C"};
            string defaultString = "A";

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValues(result, defaultString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(defaultString, result.A, "Property 'A' does not match.");
            Assert.AreEqual(defaultString, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void DefaultStringValuesIfNull_AllProperties()
        {
            // Arrange
            TestEntity result = new TestEntity { A = "G", B = "H" };

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValuesIfNull(result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("G", result.A, "Property 'A' does not match.");
            Assert.AreEqual("H", result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void DefaultStringValuesIfNull_AllProperties_DefaultA()
        {
            // Arrange
            TestEntity result = new TestEntity { A = "G", B = "H" };
            string defaultString = "A";

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValuesIfNull(result, defaultString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("G", result.A, "Property 'A' does not match.");
            Assert.AreEqual("H", result.B, "Property 'B' does not match.");
        }
      
        [TestMethod]
        public void DefaultStringValuesIfNull_OneNullProperty()
        {
            // Arrange
            TestEntity result = new TestEntity { A = "G" };

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValuesIfNull(result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("G", result.A, "Property 'A' does not match.");
            Assert.AreEqual(string.Empty, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void DefaultStringValuesIfNull_OneProperties_DefaultA()
        {
            // Arrange
            TestEntity result = new TestEntity { A = "G", };
            string defaultString = "A";

            // Act
            ClassyMapper.ClassyMapper.DefaultStringValuesIfNull(result, defaultString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("G", result.A, "Property 'A' does not match.");
            Assert.AreEqual(defaultString, result.B, "Property 'B' does not match.");
        }
      
        #endregion

        #region << ClearCacheObjects Tests >>

        [TestMethod]
        public void ClearCacheObjects()
        {
            // Arrange
            TestEntity input = new TestEntity { A = "1", B = "2" };
            ClassyMapper.ClassyMapper.New().Map<TestDto>(input);

            // Act
            ClassyMapper.ClassyMapper.ClearCacheObjects();

            // Assert
            // No exception means pass
        }

        #endregion

        #region << Big Class Test >>

        [TestMethod]
        public void LoadTest_1000_Reflection()
        {
            // Arrange
            List<BigEntity> entities = new List<BigEntity>();
            for (var i = 0; i < 1000; i++)
            {
                entities.Add(
                    new BigEntity
                    {
                        A = "1",
                        AA = "2",
                        YZAK = true,
                        YZAN = 1,
                        YZAM = i,
                        YZAL = 3,
                        YZAJ = DateTime.UtcNow,
                        YZAI = 8
                    });
            }

            // Act
            var dtos = ClassyMapper.ClassyMapper.New().MapToList<BigDto, BigEntity>(entities);

            // Assert
            Assert.AreEqual(entities.Count, dtos.Count);
            Assert.AreEqual(dtos[0].AA, entities[0].AA, "[0] AA property does not match.");
            Assert.AreEqual(dtos[10].YZAM, entities[10].YZAM, "[10] YZAM property does not match.");
            Assert.AreEqual(dtos[100].YZAM, entities[100].YZAM, "[100] YZAM property does not match.");
            Assert.AreEqual(dtos[499].YZAM, entities[499].YZAM, "[499] YZAM property does not match.");
        }

        [TestMethod]
        public void LoadTest_100000_Reflection()
        {
            // Arrange
            List<BigEntity> entities = new List<BigEntity>();
            for (var i = 0; i < 100000; i++)
            {
                entities.Add(
                    new BigEntity
                    {
                        A = "1",
                        AA = "2",
                        YZAK = true,
                        YZAN = 1,
                        YZAM = i,
                        YZAL = 3,
                        YZAJ = DateTime.UtcNow,
                        YZAI = 8
                    });
            }

            // Act
            var dtos = ClassyMapper.ClassyMapper.New().MapToList<BigDto, BigEntity>(entities);

            // Assert
            Assert.AreEqual(entities.Count, dtos.Count);
            Assert.AreEqual(dtos[0].AA, entities[0].AA, "[0] AA property does not match.");
            Assert.AreEqual(dtos[10].YZAM, entities[10].YZAM, "[10] YZAM property does not match.");
            Assert.AreEqual(dtos[100].YZAM, entities[100].YZAM, "[100] YZAM property does not match.");
            Assert.AreEqual(dtos[1000].YZAM, entities[1000].YZAM, "[1000] YZAM property does not match.");
            Assert.AreEqual(dtos[10000].YZAM, entities[10000].YZAM, "[10000] YZAM property does not match.");
            Assert.AreEqual(dtos[99999].YZAM, entities[99999].YZAM, "[99999] YZAM property does not match.");
            Assert.AreEqual(dtos[500].YZAM, entities[500].YZAM, "[500] YZAM property does not match.");
            Assert.AreEqual(dtos[5000].YZAM, entities[5000].YZAM, "[5000] YZAM property does not match.");
            Assert.AreEqual(dtos[50000].YZAM, entities[50000].YZAM, "[50000] YZAM property does not match.");
        }

        // Takes around 30 seconds
        //[TestMethod]
        //public void LoadTest_1000000_Reflection()
        //{
        //    // Arrange
        //    List<BigEntity> entities = new List<BigEntity>();
        //    for (var i = 0; i < 1000000; i++)
        //    {
        //        entities.Add(new BigEntity { A = "1", AA = "2", YZAK = true, YZAN = 1, YZAM = i, YZAL = 3, YZAJ = DateTime.UtcNow, YZAI = 8 });
        //    }

        //    // Act
        //    var watch = System.Diagnostics.Stopwatch.StartNew();
        //    var dtos = ClassyMapper.New().MapToList<BigDto, BigEntity>(entities);
        //    watch.Stop();

        //    // Assert
        //    Assert.AreEqual(entities.Count, dtos.Count);
        //    Assert.AreEqual(dtos[0].AA, entities[0].AA, "[0] AA property does not match.");
        //    Assert.AreEqual(dtos[10].YZAM, entities[10].YZAM, "[10] YZAM property does not match.");
        //    Assert.AreEqual(dtos[100].YZAM, entities[100].YZAM, "[100] YZAM property does not match.");
        //    Assert.AreEqual(dtos[1000].YZAM, entities[1000].YZAM, "[1000] YZAM property does not match.");
        //    Assert.AreEqual(dtos[10000].YZAM, entities[10000].YZAM, "[10000] YZAM property does not match.");
        //    Assert.AreEqual(dtos[999999].YZAM, entities[999999].YZAM, "[999999] YZAM property does not match.");
        //    Assert.AreEqual(dtos[500].YZAM, entities[500].YZAM, "[500] YZAM property does not match.");
        //    Assert.AreEqual(dtos[5000].YZAM, entities[5000].YZAM, "[5000] YZAM property does not match.");
        //    Assert.AreEqual(dtos[50000].YZAM, entities[50000].YZAM, "[50000] YZAM property does not match.");
        //}

        [TestMethod]
        public void LoadTest_1000_ExpressionTree()
        {
            // Arrange
            List<BigEntity> entities = new List<BigEntity>();
            for (var i = 0; i < 1000; i++)
            {
                entities.Add(
                    new BigEntity
                    {
                        A = "1",
                        AA = "2",
                        YZAK = true,
                        YZAN = 1,
                        YZAM = i,
                        YZAL = 3,
                        YZAJ = DateTime.UtcNow,
                        YZAI = 8
                    });
            }

            // Act
            var dtos =
                ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { ExpressionTreeGetSetCalls = true })
                    .MapToList<BigEntity2, BigEntity>(entities);

            // Assert
            Assert.AreEqual(entities.Count, dtos.Count);
            Assert.AreEqual(dtos[0].AA, entities[0].AA, "[0] AA property does not match.");
            Assert.AreEqual(dtos[10].YZAM, entities[10].YZAM, "[10] YZAM property does not match.");
            Assert.AreEqual(dtos[100].YZAM, entities[100].YZAM, "[100] YZAM property does not match.");
        }
        [TestMethod]
        public void LoadTest_100000_ExpressionTree()
        {
            // Arrange
            List<BigEntity> entities = new List<BigEntity>();
            for (var i = 0; i < 100000; i++)
            {
                entities.Add(
                    new BigEntity
                    {
                        A = "1",
                        AA = "2",
                        YZAK = true,
                        YZAN = 1,
                        YZAM = i,
                        YZAL = 3,
                        YZAJ = DateTime.UtcNow,
                        YZAI = 8
                    });
            }

            // Act
            var dtos =
                ClassyMapper.ClassyMapper.New(new ClassyMapperConfig {ExpressionTreeGetSetCalls = true})
                    .MapToList<BigEntity2, BigEntity>(entities);

            // Assert
            Assert.AreEqual(entities.Count, dtos.Count);
            Assert.AreEqual(dtos[0].AA, entities[0].AA, "[0] AA property does not match.");
            Assert.AreEqual(dtos[10].YZAM, entities[10].YZAM, "[10] YZAM property does not match.");
            Assert.AreEqual(dtos[100].YZAM, entities[100].YZAM, "[100] YZAM property does not match.");
            Assert.AreEqual(dtos[1000].YZAM, entities[1000].YZAM, "[1000] YZAM property does not match.");
            Assert.AreEqual(dtos[10000].YZAM, entities[10000].YZAM, "[10000] YZAM property does not match.");
            Assert.AreEqual(dtos[99999].YZAM, entities[99999].YZAM, "[99999] YZAM property does not match.");
            Assert.AreEqual(dtos[500].YZAM, entities[500].YZAM, "[500] YZAM property does not match.");
            Assert.AreEqual(dtos[5000].YZAM, entities[5000].YZAM, "[5000] YZAM property does not match.");
            Assert.AreEqual(dtos[50000].YZAM, entities[50000].YZAM, "[50000] YZAM property does not match.");
        }

        //// Takes around 20 seconds
        //[TestMethod]
        //public void LoadTest_1000000_ExpressionTree()
        //{
        //    // Arrange
        //    List<BigEntity> entities = new List<BigEntity>();
        //    for (var i = 0; i < 1000000; i++)
        //    {
        //        entities.Add(new BigEntity { A = "1", AA = "2", YZAK = true, YZAN = 1, YZAM = i, YZAL = 3, YZAJ = DateTime.UtcNow, YZAI = 8 });
        //    }

        //    // Act
        //    var watch = System.Diagnostics.Stopwatch.StartNew();
        //    var dtos = ClassyMapper.New(new ClassyMapperConfig { ExpressionTreeGetSetCalls = true }).MapToList<BigDto, BigEntity>(entities);
        //    watch.Stop();

        //    // Assert
        //    Assert.AreEqual(entities.Count, dtos.Count);
        //    Assert.AreEqual(dtos[0].AA, entities[0].AA, "[0] AA property does not match.");
        //    Assert.AreEqual(dtos[10].YZAM, entities[10].YZAM, "[10] YZAM property does not match.");
        //    Assert.AreEqual(dtos[100].YZAM, entities[100].YZAM, "[100] YZAM property does not match.");
        //    Assert.AreEqual(dtos[1000].YZAM, entities[1000].YZAM, "[1000] YZAM property does not match.");
        //    Assert.AreEqual(dtos[10000].YZAM, entities[10000].YZAM, "[10000] YZAM property does not match.");
        //    Assert.AreEqual(dtos[999999].YZAM, entities[999999].YZAM, "[999999] YZAM property does not match.");
        //    Assert.AreEqual(dtos[500].YZAM, entities[500].YZAM, "[500] YZAM property does not match.");
        //    Assert.AreEqual(dtos[5000].YZAM, entities[5000].YZAM, "[5000] YZAM property does not match.");
        //    Assert.AreEqual(dtos[50000].YZAM, entities[50000].YZAM, "[50000] YZAM property does not match.");
        //}

        [TestMethod]
        public void MapEntityToDto_LotsOfProperties_AllProperties()
        {
            // Arrange
            BigEntity input = new BigEntity { A = "1", AA = "2", YZAK = true, YZAN = 1, YZAM = 2, YZAL = 3, YZAJ = DateTime.UtcNow, YZAI = 8 };
            ClassyMapper.ClassyMapper.DefaultStringValuesIfNull(input, "Test");

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<BigDto>(input);
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            //for (int i = 0; i < 100000; i++)
            //    result = ClassyMapper.New().Map<BigDto>(input);
            //watch.Stop();
            //var t = watch.ElapsedMilliseconds;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("1", result.A, "Property 'A' does not match.");
            Assert.AreEqual("1", input.A, "Property 'A' does not match.");
            Assert.AreEqual(input.AA, result.AA, "Property 'AA' does not match.");
            Assert.AreEqual(input.YZAK, result.YZAK, "Property 'YZAK' does not match.");
            Assert.AreEqual(input.YZAN, result.YZAN, "Property 'YZAN' does not match.");
            Assert.AreEqual(input.YZAM, result.YZAM, "Property 'YZAM' does not match.");
            Assert.AreEqual(input.YZAL, result.YZAL, "Property 'YZAL' does not match.");
            Assert.AreEqual(input.YZAJ, result.YZAJ, "Property 'YZAJ' does not match.");
            Assert.AreEqual(input.YZAI, result.YZAI, "Property 'YZAI' does not match.");
            Assert.AreEqual(input.ZAD, result.ZAD, "Property 'ZAD' does not match.");
        }

        #endregion

        #region << MapClassAttribute Tests >>

        [TestMethod]
        public void MapEntityToDto_MapClass_AllProperties()
        {
            // Arrange
            MapClassEntity input = new MapClassEntity { A = "1", B = "2", C = "33", D = "32" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<MapClassDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
            Assert.AreEqual(input.D, result.D, "Property 'D' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_MapClass_AllProperties()
        {
            // Arrange
            MapClassDto input = new MapClassDto { A = "1", B = "2", C = "33", D = "32" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<MapClassEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
            Assert.AreEqual(input.D, result.D, "Property 'D' does not match.");
        }

        #endregion

        #region << MappingException Tests >>

        [TestMethod]
        [ExpectedException(typeof(MappingException))]
        public void MapEntityToDto_MappingExceptionThrown()
        {
            // Arrange
            TestEntity2 input = new TestEntity2 { C = "1" };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New(new ClassyMapperConfig {ThrowExceptionIfNoMatchingPropertyFound = true})
                    .Map<TestDto>(input);

            // Assert
            // See ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(MappingException))]
        public void MapDtoToEntity_MappingExceptionThrown()
        {
            // Arrange
            EnumDto input = new EnumDto { TesterId = 1 };

            // Act
            var result =
                ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { ThrowExceptionIfNoMatchingPropertyFound = true })
                    .Map<TestEntity2>(input);

            // Assert
            // See ExpectedException attribute
        }

        #endregion

        #region << Parent/Child Reference Tests >>

        [TestMethod]
        public void MapEntityToDto_ParentChildReference_AllProperties()
        {
            // Arrange
            ParentEntity input = new ParentEntity { Test = "1", Children = new List<ChildEntity>() };
            input.Children.Add(new ChildEntity { ChildA = "A", Parent = input });
            input.Children.Add(new ChildEntity { ChildA = "B", Parent = input });

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ParentDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.Test, result.Test, "Property 'A' does not match.");
            Assert.AreEqual(input.Children.Count, result.Children.Count, "Property 'Children.Count' does not match.");
            Assert.AreEqual(input.Children.First().ChildA, result.Children.First().ChildA, "Property 'Children[0].ChildA' does not match.");
            Assert.AreEqual(result, result.Children.First().Parent, "Property 'Children[0].Parent' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_ParentChildReference_AllProperties()
        {
            // Arrange
            ParentDto input = new ParentDto { Test = "1", Children = new List<ChildDto>() };
            input.Children.Add(new ChildDto { ChildA = "A", Parent = input });
            input.Children.Add(new ChildDto { ChildA = "B", Parent = input });

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ParentEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.Test, result.Test, "Property 'A' does not match.");
            Assert.AreEqual(input.Children.Count, result.Children.Count, "Property 'Children.Count' does not match.");
            Assert.AreEqual(input.Children.First().ChildA, result.Children.First().ChildA, "Property 'Children[0].ChildA' does not match.");
            Assert.AreEqual(result, result.Children.First().Parent, "Property 'Children[0].Parent' does not match.");
        }

        #endregion

        #region << No Parameterless Constructor Tests >>

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void MapEntityToDto_NoParameterlessConstructor_AllProperties_NoConstructor()
        {
            // Arrange
            NoParameterlessConstructorEntity input = new NoParameterlessConstructorEntity("1");

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<NoParameterlessConstructorDto>(input);

            // Assert
            // See ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void MapDtoToEntity_NoParameterlessConstructor_AllProperties_NoConstructor()
        {
            // Arrange
            NoParameterlessConstructorDto input = new NoParameterlessConstructorDto("1");
            
            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<NoParameterlessConstructorEntity>(input);

            // Assert
            // See ExpectedException attribute
        }

        [TestMethod]
        public void MapEntityToDto_NoParameterlessConstructor_AllProperties_ConstructorRegistered()
        {
            // Arrange
            NoParameterlessConstructorEntity input = new NoParameterlessConstructorEntity("1");

            // Act
            var result =
                ClassyMapper.ClassyMapper.New()
                    .RegisterConstructor<NoParameterlessConstructorEntity, NoParameterlessConstructorDto>(
                        from => new NoParameterlessConstructorDto("2"))
                    .Map<NoParameterlessConstructorDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_NoParameterlessConstructor_AllProperties_ConstructorRegistered()
        {
            // Arrange
            NoParameterlessConstructorDto input = new NoParameterlessConstructorDto("1");

            // Act
            var result =
                ClassyMapper.ClassyMapper.New()
                    .RegisterConstructor<NoParameterlessConstructorDto, NoParameterlessConstructorEntity>(
                        from => new NoParameterlessConstructorEntity("2"))
                    .Map<NoParameterlessConstructorEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
        }

        #endregion

        #region << Struct Tests >>

        [TestMethod]
        public void MapEntityToDto_Struct_AllProperties()
        {
            // Arrange
            StructEntity input = new StructEntity { Id = 5 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<StructDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.Id, result.Id, "Property 'Id' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_Struct_AllProperties()
        {
            // Arrange
            StructDto input = new StructDto { Id = 55 };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<StructEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.Id, result.Id, "Property 'Id' does not match.");
        }

        [TestMethod]
        public void MapEntityToDto_Struct_AllProperties_ExpressionWay()
        {
            // Arrange
            StructEntity input = new StructEntity { Id = 555 };

            // Act
            var result = ClassyMapper.ClassyMapper.New(new ClassyMapperConfig { ExpressionTreeGetSetCalls = true })
                .Map<StructDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.Id, result.Id, "Property 'Id' does not match.");
        }

        #endregion

        #region << Ihnerit Test >>

        [TestMethod]
        public void MapEntityToDto_Inherit_AllProperties()
        {
            // Arrange
            InheritTestEntity input = new InheritTestEntity { A = "1", B = "2", C = "3"};

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<InheritTestDto2>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_Inherit_AllProperties()
        {
            // Arrange
            InheritTestDto2 input = new InheritTestDto2 { A = "1", B = "2", C = "3" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<InheritTestEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
        }
        
        [TestMethod]
        public void MapEntityToDto_InheritAll_AllProperties()
        {
            // Arrange
            InheritTestEntity input = new InheritTestEntity { A = "1", B = "2", C = "3"};

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<InheritTestDtoAll2>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_InheritAll_AllProperties()
        {
            // Arrange
            InheritTestDtoAll2 input = new InheritTestDtoAll2 { A = "1", B = "2", C = "3" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<InheritTestEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
            Assert.AreEqual(input.C, result.C, "Property 'C' does not match.");
        }

        #endregion

        #region << No Attributes Test >>

        [TestMethod]
        public void MapEntityToDto_NoAttributes_AllProperties()
        {
            // Arrange
            SomeEntity input = new SomeEntity { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<SomeDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_NoAttributes_AllProperties()
        {
            // Arrange
            SomeDto input = new SomeDto { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<SomeEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.A, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        #endregion

        #region << ExcludeProperty Test >>

        [TestMethod]
        public void MapEntityToDto_ExcludeProperty_AllProperties()
        {
            // Arrange
            ExcludeEntity input = new ExcludeEntity { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ExcludeDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        [TestMethod]
        public void MapDtoToEntity_ExcludeProperty_AllProperties()
        {
            // Arrange
            ExcludeDto input = new ExcludeDto { A = "1", B = "2" };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ExcludeEntity>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.A, "Property 'A' does not match.");
            Assert.AreEqual(input.B, result.B, "Property 'B' does not match.");
        }

        #endregion

        #region << Map All None Test >>

        [TestMethod]
        public void MapDtoToEntity_MapAllNone()
        {
            // Arrange
            MapAllNoneEntity input = new MapAllNoneEntity
            {
                A = "1",
                B = "2"
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<MapAllNoneDto>(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(input.A, result.A, "Property 'A' does match.");
            Assert.AreNotEqual(input.B, result.B, "Property 'B' does match.");
        }

        #endregion

        #region << Direct Reference of Non Primitive Test >>

        
        [TestMethod]
        public void MapDtoToEntity_DirectNonPrimitiveReference_Equal()
        {
            // Arrange
            ChildEntity input = new ChildEntity
            {
                ChildA = "A",
                Parent = new ParentEntity {  Test = "B"}
            };
            ChildDto dto = new ChildDto
            {
                ChildA = "",
                Parent = new ParentDto { Test = "" }
            };

            // Act
            var result = ClassyMapper.ClassyMapper.New().Map<ChildDto>(dto, input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(input.ChildA, input.ChildA, "Property 'ChildA' does match.");
            Assert.AreEqual(input.Parent.Test, result.Parent.Test, "Property 'Parent.Test' does match.");
            Assert.AreEqual(dto.Parent, result.Parent, "Parent reference is not identical.");
        }
        
        #endregion

        #region << Private Classes >>

        private class MapAllNoneEntity
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        [MapAllProperties(MapAllPropertiesTypeEnum.None)]
        private class MapAllNoneDto
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        private class ExcludeEntity
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        private class ExcludeDto
        {
            [MapProperty(mapPropertyType: MapPropertyTypeEnum.Exclude)]
            public string A { get; set; }
            [MapProperty]
            public string B { get; set; }
        }

        private class SomeEntity
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        private class SomeDto
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        private class InheritTestEntity
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }

        private class InheritTestDto
        {
            [MapProperty]
            public string A { get; set; }
            [MapProperty]
            public string B { get; set; }
        }

        private class InheritTestDto2 : InheritTestDto
        {
            [MapProperty]
            public string C { get; set; }
        }

        [MapAllProperties]
        private class InheritTestDtoAll
        {
            [MapProperty]
            public string A { get; set; }
            public string B { get; set; }
        }

        [MapAllProperties(MapAllPropertiesTypeEnum.BaseTypeOnly, typeof(InheritTestDtoAll))]
        private class InheritTestDtoAll2 : InheritTestDtoAll
        {
            [MapProperty]
            public string C { get; set; }
        }


        private struct StructEntity
        {

            public int Id { get; set; }

        }

        [MapAllProperties]
        private struct StructDto
        {

            [MapProperty]
            public int Id { get; set; }

        }

        private class NoParameterlessConstructorEntity
        {
            public string A { get; set; }

            public NoParameterlessConstructorEntity(string a)
            {
                A = a;
            }
        }

        private class NoParameterlessConstructorDto
        {
            [MapProperty]
            public string A { get; set; }

            public NoParameterlessConstructorDto(string a)
            {
                A = a;
            }
        }

        private class ParentEntity
        {
            public string Test { get; set; }
            public ICollection<ChildEntity> Children { get; set; }
        }

        private class ChildEntity
        {
            public string ChildA { get; set; }
            public ParentEntity Parent { get; set; }
        }

        [MapAllProperties]
        private class ParentDto
        {
            public string Test { get; set; }
            public IList<ChildDto> Children { get; set; }
        }

        [MapAllProperties]
        private class ChildDto
        {
            public string ChildA { get; set; }
            public ParentDto Parent { get; set; }
        }


        private class MapClassEntity 
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
        }

        [MapAllProperties]
        private class MapClassDto
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
        }
        private class BigEntity2
        {
            public string A { get; set; }
            public string AA { get; set; }
            public string AB { get; set; }
            public string AC { get; set; }
            public string AD { get; set; }
            public string AE { get; set; }
            public string AF { get; set; }
            public string AG { get; set; }
            public string AH { get; set; }
            public string AI { get; set; }
            public string AJ { get; set; }
            public string AK { get; set; }
            public string AL { get; set; }
            public string AM { get; set; }
            public string AN { get; set; }
            public string ZA { get; set; }
            public string ZAA { get; set; }
            public string ZAB { get; set; }
            public string ZAC { get; set; }
            public string ZAD { get; set; }
            public string ZAE { get; set; }
            public string ZAF { get; set; }
            public string ZAG { get; set; }
            public string ZAH { get; set; }
            public string ZAI { get; set; }
            public string ZAJ { get; set; }
            public string ZAK { get; set; }
            public string ZAL { get; set; }
            public string ZAM { get; set; }
            public string ZAN { get; set; }
            public string YA { get; set; }
            public string YAA { get; set; }
            public string YAB { get; set; }
            public string YAC { get; set; }
            public string YAD { get; set; }
            public string YAE { get; set; }
            public string YAF { get; set; }
            public string YAG { get; set; }
            public string YAH { get; set; }
            public string YAI { get; set; }
            public string YAJ { get; set; }
            public string YAK { get; set; }
            public string YAL { get; set; }
            public string YAM { get; set; }
            public string YAN { get; set; }
            public string YZA { get; set; }
            public string YZAA { get; set; }
            public string YZAB { get; set; }
            public string YZAC { get; set; }
            public string YZAD { get; set; }
            public string YZAE { get; set; }
            public string YZAF { get; set; }
            public string YZAG { get; set; }
            public string YZAH { get; set; }
            public long? YZAI { get; set; }
            public DateTime YZAJ { get; set; }
            public bool YZAK { get; set; }
            public long YZAL { get; set; }
            public int YZAM { get; set; }
            public byte YZAN { get; set; }
        }

        private class BigEntity
        {
            public string A { get; set; }
            public string AA { get; set; }
            public string AB { get; set; }
            public string AC { get; set; }
            public string AD { get; set; }
            public string AE { get; set; }
            public string AF { get; set; }
            public string AG { get; set; }
            public string AH { get; set; }
            public string AI { get; set; }
            public string AJ { get; set; }
            public string AK { get; set; }
            public string AL { get; set; }
            public string AM { get; set; }
            public string AN { get; set; }
            public string ZA { get; set; }
            public string ZAA { get; set; }
            public string ZAB { get; set; }
            public string ZAC { get; set; }
            public string ZAD { get; set; }
            public string ZAE { get; set; }
            public string ZAF { get; set; }
            public string ZAG { get; set; }
            public string ZAH { get; set; }
            public string ZAI { get; set; }
            public string ZAJ { get; set; }
            public string ZAK { get; set; }
            public string ZAL { get; set; }
            public string ZAM { get; set; }
            public string ZAN { get; set; }
            public string YA { get; set; }
            public string YAA { get; set; }
            public string YAB { get; set; }
            public string YAC { get; set; }
            public string YAD { get; set; }
            public string YAE { get; set; }
            public string YAF { get; set; }
            public string YAG { get; set; }
            public string YAH { get; set; }
            public string YAI { get; set; }
            public string YAJ { get; set; }
            public string YAK { get; set; }
            public string YAL { get; set; }
            public string YAM { get; set; }
            public string YAN { get; set; }
            public string YZA { get; set; }
            public string YZAA { get; set; }
            public string YZAB { get; set; }
            public string YZAC { get; set; }
            public string YZAD { get; set; }
            public string YZAE { get; set; }
            public string YZAF { get; set; }
            public string YZAG { get; set; }
            public string YZAH { get; set; }
            public long? YZAI { get; set; }
            public DateTime YZAJ { get; set; }
            public bool YZAK { get; set; }
            public long YZAL { get; set; }
            public int YZAM { get; set; }
            public byte YZAN { get; set; }
        }
        private class BigDto
        {
            [MapProperty]
            public string A { get; set; }
            [MapProperty]
            public string AA { get; set; }
            [MapProperty]
            public string AB { get; set; }
            [MapProperty]
            public string AC { get; set; }
            [MapProperty]
            public string AD { get; set; }
            [MapProperty]
            public string AE { get; set; }
            [MapProperty]
            public string AF { get; set; }
            [MapProperty]
            public string AG { get; set; }
            [MapProperty]
            public string AH { get; set; }
            [MapProperty]
            public string AI { get; set; }
            [MapProperty]
            public string AJ { get; set; }
            [MapProperty]
            public string AK { get; set; }
            [MapProperty]
            public string AL { get; set; }
            [MapProperty]
            public string AM { get; set; }
            [MapProperty]
            public string AN { get; set; }
            [MapProperty]
            public string ZA { get; set; }
            [MapProperty]
            public string ZAA { get; set; }
            [MapProperty]
            public string ZAB { get; set; }
            [MapProperty]
            public string ZAC { get; set; }
            [MapProperty]
            public string ZAD { get; set; }
            [MapProperty]
            public string ZAE { get; set; }
            [MapProperty]
            public string ZAF { get; set; }
            [MapProperty]
            public string ZAG { get; set; }
            [MapProperty]
            public string ZAH { get; set; }
            [MapProperty]
            public string ZAI { get; set; }
            [MapProperty]
            public string ZAJ { get; set; }
            [MapProperty]
            public string ZAK { get; set; }
            [MapProperty]
            public string ZAL { get; set; }
            [MapProperty]
            public string ZAM { get; set; }
            [MapProperty]
            public string ZAN { get; set; }
            [MapProperty]
            public string YA { get; set; }
            [MapProperty]
            public string YAA { get; set; }
            [MapProperty]
            public string YAB { get; set; }
            [MapProperty]
            public string YAC { get; set; }
            [MapProperty]
            public string YAD { get; set; }
            [MapProperty]
            public string YAE { get; set; }
            [MapProperty]
            public string YAF { get; set; }
            [MapProperty]
            public string YAG { get; set; }
            [MapProperty]
            public string YAH { get; set; }
            [MapProperty]
            public string YAI { get; set; }
            [MapProperty]
            public string YAJ { get; set; }
            [MapProperty]
            public string YAK { get; set; }
            [MapProperty]
            public string YAL { get; set; }
            [MapProperty]
            public string YAM { get; set; }
            [MapProperty]
            public string YAN { get; set; }
            [MapProperty]
            public string YZA { get; set; }
            [MapProperty]
            public string YZAA { get; set; }
            [MapProperty]
            public string YZAB { get; set; }
            [MapProperty]
            public string YZAC { get; set; }
            [MapProperty]
            public string YZAD { get; set; }
            [MapProperty]
            public string YZAE { get; set; }
            [MapProperty]
            public string YZAF { get; set; }
            [MapProperty]
            public string YZAG { get; set; }
            [MapProperty]
            public string YZAH { get; set; }
            [MapProperty]
            public long? YZAI { get; set; }
            [MapProperty]
            public DateTime YZAJ { get; set; }
            [MapProperty]
            public bool YZAK { get; set; }
            [MapProperty]
            public long YZAL { get; set; }
            [MapProperty]
            public int YZAM { get; set; }
            [MapProperty]
            public byte YZAN { get; set; }
        }

        private class SubListEntity
        {
            public string A { get; set; }
            public List<SubListEntity2> List { get; set; }
        }

        public class SubListEntity2
        {
            public string A { get; set; }
        }

        private class SubListDto
        {
            [MapProperty]
            public string A { get; set; }
            [MapProperty]
            public List<SubListDto2> List { get; set; }
        }

        public class SubListDto2
        {
            [MapProperty]
            public string A { get; set; }
        }

        private class TimestampEntity
        {
            public byte[] Timestamp { get; set; }
        }

        private class TimestampDto
        {
            [MapProperty(isTimesatmp: true)]
            public string Timestamp { get; set; }
        }

        private class NamespaceEntity
        {
            public string A { get; set; }
        }

        private class NamespaceEntity2
        {
            public string A { get; set; }
        }

        private class NamespaceDto
        {
            [MapProperty(fullName: "ClassyTest.ClassyMapperTest+NamespaceEntity")]
            public string A { get; set; }

            [MapProperty("A", fullName: "ClassyTest.ClassyMapperTest+NamespaceEntity2")]
            public string AA { get; set; }
        }

        private class UpCastEntity
        {
            public int Test { get; set; }
        }

        private class UpCastDto
        {
            [MapProperty]
            public long Test { get; set; }
        }

        public class NullableEntity
        {
            public int? A { get; set; }
            public DateTime? B { get; set; }
            public int C { get; set; }
            public DateTime D { get; set; }
        }


        public class NullableDto
        {
            [MapProperty]
            public int A { get; set; }

            [MapProperty]
            public DateTime B { get; set; }

            [MapProperty]
            public int? C { get; set; }

            [MapProperty]
            public DateTime? D { get; set; }
        }

        private enum TestEnum : int
        {
            Unknown = 0,
            MyValue = 1,
            YourValue = 2
        }

        private class EnumEntity
        {

            public long TesterId { get; set; }
            public int MyTest { get; set; }
            public int Check { get; set; }
        }

        private class EnumEntity2
        {

            public long TesterId { get; set; }
            public string MyTest { get; set; }
            public string Check { get; set; }

        }
        private class EnumDto
        {

            [MapProperty]
            public long TesterId { get; set; }
            
            [MapProperty]
            public TestEnum MyTest { get; set; }

            [MapProperty]
            public TestEnum? Check { get; set; }
        }


        private class RecursionParentEntity
        {
            public string A { get; set; }
            public RecursionChildEntity Child { get; set; }
        }
        private class RecursionChildEntity
        {
            public string B { get; set; }
            public RecursionParentEntity Parent { get; set; }
        }

        private class RecursionParentDto : IIsNullable
        {
            [MapProperty]
            public string A { get; set; }

            [MapProperty]
            public RecursionChildDto Child { get; set; }

            public bool IsNull { get; set; }

        }
        private class RecursionChildDto : IIsNullable
        {
            [MapProperty]
            public string B { get; set; }

            [MapProperty]
            public RecursionParentDto Parent { get; set; }

            public bool IsNull { get; set; }
        }



        private class ComplexEntity
        {
            public int A { get; set; }
            public int B { get; set; }
            public ComplexEntityInner Inner { get; set; }
        }

        private class ComplexEntityInner
        {
            public string B { get; set; }
        }


        private class ComplexDto
        {
            [MapProperty]
            public int A { get; set; }

            [MapProperty]
            public string B { get; set; }

            [MapProperty]
            public ComplexDtoInner Inner { get; set; }
        }

        private class ComplexDtoInner : IIsNullable
        {
            [MapProperty]
            public string B { get; set; }

            public bool IsNull { get; set; }
        }



        private class TestEntity
        {

            public string A { get; set; }
            public string B { get; set; }

            public TestEntityInner Inner { get; set; }
        }


        private class TestEntityInner
        {

            public string D { get; set; }

        }

        private class TestEntity2
        {

            public string C { get; set; }

        }

        private class TestDto
        {

            [MapProperty]
            public string A { get; set; }

            [MapProperty]
            public string B { get; set; }

            [MapProperty]
            public string C { get; set; }

            [MapProperty("D")]
            public string F { get; set; }

        }

        #endregion

    }
}