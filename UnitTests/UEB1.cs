﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BIF.SWE1.Interfaces;

namespace BIF.SWE1.UnitTests
{
    [TestFixture]
    public class UEB1 : AbstractTestFixture<Uebungen.UEB1>
    {
        [Test]
        public void HelloWorld()
        {
            var ueb = CreateInstance();
            ueb.HelloWorld();
        }

        #region Url
        [Test]
        public void url_should_create_empty()
        {
            var obj = CreateInstance().GetUrl(null);
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(string.IsNullOrEmpty(obj.Path), Is.True);
        }

        [Test]
        public void url_should_return_raw_url_0()
        {
            var obj = CreateInstance().GetUrl("/test.jpg");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.RawUrl, Is.EqualTo("/test.jpg"));
        }

        [Test]
        public void url_should_return_raw_url_1()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=y");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.RawUrl, Is.EqualTo("/test.jpg?x=y"));
        }

        [Test]
        public void url_should_return_raw_url_2()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=1&y=2");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.RawUrl, Is.EqualTo("/test.jpg?x=1&y=2"));
        }

        [Test]
        public void url_should_create_with_path()
        {
            var obj = CreateInstance().GetUrl("/test.jpg");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.Path, Is.EqualTo("/test.jpg"));
        }

        [Test]
        public void url_should_parse_parameter()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=1");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.Parameter.ContainsKey("x"), Is.True);
            Assert.That(obj.Parameter["x"], Is.EqualTo("1"));
        }

        [Test]
        public void url_should_parse_more_parameter()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=1&y=2");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.Parameter.ContainsKey("x"), Is.True);
            Assert.That(obj.Parameter["x"], Is.EqualTo("1"));
            Assert.That(obj.Parameter.ContainsKey("y"), Is.True);
            Assert.That(obj.Parameter["y"], Is.EqualTo("2"));
        }

        [Test]
        public void url_should_parse_return_path_without_parameter()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=1");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");
            
            Assert.That(obj.Path, Is.EqualTo("/test.jpg"));
        }

        [Test]
        public void url_should_count_parameter()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=7");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.ParameterCount, Is.EqualTo(1));
        }

        [Test]
        public void url_should_count_parameter_2()
        {
            var obj = CreateInstance().GetUrl("/test.jpg?x=7&y=foo");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.ParameterCount, Is.EqualTo(2));
        }

        [Test]
        public void url_should_count_parameter_0()
        {
            var obj = CreateInstance().GetUrl("/test.jpg");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.ParameterCount, Is.EqualTo(0));
        }

        [Test]
        public void url_should_count_parameter_empty()
        {
            var obj = CreateInstance().GetUrl(null);
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.ParameterCount, Is.EqualTo(0));
        }
        #endregion

        #region CustomTests
        [Test]
        public void url_should_parse_segments()
        {
            var obj = CreateInstance().GetUrl("/public/test.jpg");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.Segments[0], Is.EqualTo("public"));
            Assert.That(obj.Segments[1], Is.EqualTo("test.jpg"));
        }
        [Test]
        public void url_should_parse_filename()
        {
            var obj = CreateInstance().GetUrl("/test.jpg");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.FileName, Is.EqualTo("test.jpg"));
        }
        [Test]
        public void url_should_parse_extension()
        {
            var obj = CreateInstance().GetUrl("/test.jpg");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.Extension, Is.EqualTo(".jpg"));
        }
        [Test]
        public void url_should_parse_fragment()
        {
            var obj = CreateInstance().GetUrl("/test.jpg#fragment1");
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.Fragment, Is.EqualTo("fragment1"));
        }
        [Test]
        public void url_should_never_return_null()
        {
            var obj = CreateInstance().GetUrl(null);
            Assert.That(obj, Is.Not.Null, "IUEB1.GetUrl returned null");

            Assert.That(obj.Parameter, Is.Not.Null, "Parameter returned null");
            Assert.That(obj.Segments, Is.Not.Null, "Segments returned null");
            Assert.That(obj.FileName, Is.Not.Null, "Filename returned null");
            Assert.That(obj.Extension, Is.Not.Null, "Extension returned null");
            Assert.That(obj.Fragment, Is.Not.Null, "Fragment returned null");
        }
        #endregion
    }
}
