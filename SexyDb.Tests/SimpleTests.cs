﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SexyDb.Tests
{
    [TestFixture]
    public class SimpleTests 
    {
        [Test]
        public async Task SaveStringProperty()
        {
            var db = new StringPropertyDatabase();
            db.StringObject = new StringPropertyObject { StringProperty = "foo" };
            await db.WaitForIdle();

            var node = (DbValuePropertyNode)((ISexyDatabase)db).Node.EvaluatePath(nameof(StringPropertyDatabase.StringObject), nameof(StringPropertyObject.StringProperty));
            var value = File.ReadAllText(node.File.FullName);
            Assert.AreEqual(db.StringObject.StringProperty, value);
        }

        [Test]
        public async Task StringPropertySaved()
        {
            var db = new StringPropertyDatabase { StringObject = new StringPropertyObject() };
            db.StringObject.StringProperty = "foo";
            await db.WaitForIdle();

            var node = (DbValuePropertyNode)((ISexyDatabase)db).Node.EvaluatePath(nameof(StringPropertyDatabase.StringObject), nameof(StringPropertyObject.StringProperty));
            var value = File.ReadAllText(node.File.FullName);
            Assert.AreEqual(db.StringObject.StringProperty, value);
        }

        [Test]
        public async Task LoadStringProperty()
        {
            var db = new StringPropertyDatabase { StringObject = new StringPropertyObject() };
            var node = (DbValuePropertyNode)((ISexyDatabase)db).Node.EvaluatePath(nameof(StringPropertyDatabase.StringObject), nameof(StringPropertyObject.StringProperty));
            await db.EditFile(node.File, "foo");

            Assert.AreEqual("foo", db.StringObject.StringProperty);
        }

        [Test]
        public async Task StringPropertyLoaded()
        {
            var db = new StringPropertyDatabase();
            db.StringObject = new StringPropertyObject { StringProperty = "foo" };
            await db.WaitForIdle();

            var node = (DbValuePropertyNode)((ISexyDatabase)db).Node.EvaluatePath(nameof(StringPropertyDatabase.StringObject), nameof(StringPropertyObject.StringProperty));
            await db.EditFile(node.File, "bar");

            Assert.AreEqual("bar", db.StringObject.StringProperty);
        }

        public class StringPropertyObject : BaseTestObject
        {
            public string StringProperty { get; set; }
        }

        public class StringPropertyDatabase : TestDatabase
        {
            public StringPropertyObject StringObject { get; set; }
        }

        [Test]
        public async Task SaveIntProperty()
        {
            var db = new IntPropertyDatabase();
            db.IntProperty = 5;
            await db.WaitForIdle();

            var value = int.Parse(File.ReadAllText(((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File.FullName));
            Assert.AreEqual(db.IntProperty, value);
        }

        [Test]
        public async Task LoadIntProperty()
        {
            var db = new IntPropertyDatabase();
            db.IntProperty = 5;
            await db.WaitForIdle();

            var file = ((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File;
            await db.EditFile(file, "4");

            Assert.AreEqual(4, db.IntProperty);
        }

        [Test]
        public async Task LoadIntPropertyUninitialized()
        {
            var db = new IntPropertyDatabase();

            var file = ((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File;
            await db.EditFile(file, "4");

            Assert.AreEqual(4, db.IntProperty);
        }

        public class IntPropertyDatabase : TestDatabase
        {
            public int IntProperty { get; set; }
        }

        [Test]
        public async Task SaveDateTimeProperty()
        {
            var db = new DateTimePropertyDatabase();
            db.DateTimeProperty = new DateTime(2001, 2, 3, 4, 5, 6);
            await db.WaitForIdle();

            var value = DateTime.ParseExact(File.ReadAllText(((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File.FullName), "o", null);
            Assert.AreEqual(db.DateTimeProperty, value);
        }

        [Test]
        public async Task LoadDateTimeProperty()
        {
            var db = new DateTimePropertyDatabase();
            db.DateTimeProperty = new DateTime(2001, 2, 3, 4, 5, 6);
            await db.WaitForIdle();

            var file = ((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File;
            await db.EditFile(file, new DateTime(2002, 3, 4, 5, 6, 7).ToString("o"));

            Assert.AreEqual(new DateTime(2002, 3, 4, 5, 6, 7), db.DateTimeProperty);
        }

        public class DateTimePropertyDatabase : TestDatabase
        {
            public DateTime DateTimeProperty { get; set; }
        }

        [Test]
        public async Task SaveEnumProperty()
        {
            var db = new EnumPropertyDatabase();
            db.EnumProperty = TestEnum.Value2;
            await db.WaitForIdle();

            var value = File.ReadAllText(((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File.FullName);
            Assert.AreEqual(db.EnumProperty.ToString(), value);
        }

        [Test]
        public async Task LoadEnumProperty()
        {
            var db = new EnumPropertyDatabase();
            db.EnumProperty = TestEnum.Value2;
            await db.WaitForIdle();

            var file = ((ISexyDatabase)db).Node.PropertyNodes.Values.Cast<DbValuePropertyNode>().Single().File;
            await db.EditFile(file, TestEnum.Value1.ToString());

            Assert.AreEqual(TestEnum.Value1, db.EnumProperty);
        }

        public class EnumPropertyDatabase : TestDatabase
        {
            public TestEnum EnumProperty { get; set; }
        }

        public enum TestEnum
        {
            Value1, Value2
        }
    }
}