using System;
using System.IO;
using System.Linq;
using Xunit;
using Library;
using System.Globalization;

namespace Library.Tests
{
    public class DomToolkidTests : IDisposable
    {
        private readonly string _testFilePath = "test_transistors.xml";
        private readonly DomToolkid _domToolkid;

        public DomToolkidTests()
        {
            // Создаем тестовый XML файл
            File.WriteAllText(_testFilePath, "<root></root>");
            _domToolkid = new DomToolkid(_testFilePath);
        }

        public void Dispose()
        {
            // Удаляем тестовый файл после каждого теста
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public void Constructor_ShouldCreateDocument_WhenFileExists()
        {
            Assert.NotNull(_domToolkid._doc);
            Assert.Equal(_testFilePath, _domToolkid._path);
        }

        [Fact]
        public void Constructor_ShouldCreateNewDocument_WhenFileNotExists()
        {
            var nonExistentPath = "nonexistent.xml";
            var toolkid = new DomToolkid(nonExistentPath);
            
            Assert.NotNull(toolkid._doc);
            File.Delete(nonExistentPath);
        }

        [Fact]
        public void NodeCreate_ShouldAddNewTransistor()
        {
            var transistor = CreateTestTransistor(1);
            _domToolkid.nodeCreate(transistor);

            var loaded = _domToolkid.nodeShow(1);
            Assert.Equal(transistor._id, loaded._id);
        }

        [Fact]
        public void NodeShow_ShouldReturnNull_ForNonExistentId()
        {
            var result = _domToolkid.nodeShow(999);
            Assert.Null(result);
        }

        [Fact]
        public void NodeRemove_ShouldDeleteTransistor()
        {
            var transistor = CreateTestTransistor(1);
            _domToolkid.nodeCreate(transistor);
            
            _domToolkid.nodeRemove(1);
            
            var result = _domToolkid.nodeShow(1);
            Assert.Null(result);
        }

        [Fact]
        public void NodeRemove_ShouldNotThrow_ForNonExistentId()
        {
            var exception = Record.Exception(() => _domToolkid.nodeRemove(999));
            Assert.Null(exception);
        }

        [Fact]
        public void NodeUpdate_ShouldModifyExistingTransistor()
        {
            var original = CreateTestTransistor(1);
            _domToolkid.nodeCreate(original);

            var updated = new Transistor(
                1, 
                "UpdatedName",
                new List<TransistorType> { TransistorType.MOSFET, TransistorType.IGBT },
                12.5f, 
                0.8f, 
                2.99f, 
                "Germany");

            _domToolkid.nodeUpdate(1, updated);

            var result = _domToolkid.nodeShow(1);
            Assert.Equal("UpdatedName", result._name);
            Assert.Contains(TransistorType.MOSFET, result._types);
        }

        [Fact]
        public void SaveDoc_ShouldPersistChangesToFile()
        {
            var transistor = CreateTestTransistor(1);
            _domToolkid.nodeCreate(transistor);
            _domToolkid.saveDoc();

            var fileContent = File.ReadAllText(_testFilePath);
            Assert.Contains("<transistor id=\"001\">", fileContent);
        }

        [Fact]
        public void NodeCreate_ShouldGenerateCorrectIdFormat()
        {
            var transistor = CreateTestTransistor(1);
            _domToolkid.nodeCreate(transistor);

            var fileContent = File.ReadAllText(_testFilePath);
            Assert.Contains("id=\"001\"", fileContent);
        }

        [Fact]
        public void MultipleNodeCreates_ShouldWorkCorrectly()
        {
            for (int i = 1; i <= 5; i++)
            {
                _domToolkid.nodeCreate(CreateTestTransistor(i));
            }

            for (int i = 1; i <= 5; i++)
            {
                var result = _domToolkid.nodeShow(i);
                Assert.Equal(i, result._id);
            }
        }

        [Fact]
        public void NodeCreate_ShouldHandleEmptyTypesList()
        {
            var transistor = new Transistor(
                1, "Test", 
                new List<TransistorType>(), 
                12.5f, 0.8f, 2.99f, "USA");
            
            var exception = Record.Exception(() => _domToolkid.nodeCreate(transistor));
            Assert.Null(exception);
        }

        [Fact]
        public void NodeUpdate_ShouldChangeId_WhenNewIdProvided()
        {
            var original = CreateTestTransistor(1);
            _domToolkid.nodeCreate(original);

            var updated = CreateTestTransistor(2);
            _domToolkid.nodeUpdate(1, updated);

            Assert.Null(_domToolkid.nodeShow(1));
            Assert.NotNull(_domToolkid.nodeShow(2));
        }

        [Fact]
        public void NodeCreate_ShouldThrow_ForNullTransistor()
        {
            Assert.Throws<NullReferenceException>(() => _domToolkid.nodeCreate(null));
        }

        private Transistor CreateTestTransistor(int id)
        {
            return new Transistor(
                id,
                "TestTransistor",
                new List<TransistorType> { TransistorType.BJT, TransistorType.MOSFET },
                12.0f,
                0.5f,
                1.99f,
                "USA");
        }
    }
}