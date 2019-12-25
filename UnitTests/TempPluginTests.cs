using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using NUnit.Framework;
using TempPlugin;

namespace BIF.SWE1.UnitTests
{
    [TestFixture]
    public class TempPluginTests : AbstractTestFixture<Uebungen.UEB6>
    {
        private readonly string _dbConnectionString = "Host=localhost;Username=swe;Password=123456;Database=webserver_test";

        [Test]
        public void temp_plugin_handle_custom()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetTemperaturePlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetTemperaturePlugin returned null");

            var url = ueb.GetTemperatureUrl(new DateTime(2014, 1, 1), new DateTime(2014, 1, 2));
            Assert.That(url, Is.Not.Null, "IUEB6.GetTemperatureUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentType, Is.EqualTo("text/html; charset=UTF-8"));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public void temp_plugin_handle_rest_call_custom()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetTemperaturePlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetTemperaturePlugin returned null");

            var url = ueb.GetTemperatureRestUrl(new DateTime(2014, 1, 1), new DateTime(2014, 1, 2));
            Assert.That(url, Is.Not.Null, "IUEB6.GetTemperatureUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentType, Is.EqualTo("application/json"));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public void db_connection_is_open()
        {
            NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString);
            Assert.That(connection, Is.Not.Null);

            connection.Open();
            Assert.AreEqual(ConnectionState.Open, connection.State);
        }

        [Test]
        public void temp_controller_add_get_remove_count()
        {
            var tempController = new TempPlugin.TempController(_dbConnectionString);

            var addEntity = new TempPlugin.TempModel
            {
                Id = 1234,
                Value = 1.12345f,
                DateTime = new DateTime(2019, 12, 1, 12, 0, 0)
            };
            var addSuccess = tempController.AddTemp(addEntity);
            Assert.IsTrue(addSuccess);

            addSuccess = tempController.AddTemp(addEntity);
            Assert.IsFalse(addSuccess);

            var count = tempController.Count();
            Assert.AreEqual(1, count);

            var getEntity = tempController.GetTemp((int) addEntity.Id);
            Assert.IsNotNull(getEntity);
            Assert.AreEqual(addEntity.Id, getEntity.Id);
            Assert.AreEqual(addEntity.DateTime, getEntity.DateTime);
            Assert.AreEqual(addEntity.Value, getEntity.Value);

            var removeSuccess = tempController.RemoveTemp((int) addEntity.Id);
            Assert.IsTrue(removeSuccess);
            
            removeSuccess = tempController.RemoveTemp((int) addEntity.Id);
            Assert.IsFalse(removeSuccess);
            
            getEntity = tempController.GetTemp((int) addEntity.Id);
            Assert.IsNull(getEntity);
            
            count = tempController.Count();
            Assert.AreEqual(0, count);

            /*
             Test without Id
             however can not be removed because we dont know the id
             
            var testEntity = new TempPlugin.TempModel
            {
                Value =  1.12345f,
                DateTime = new DateTime(2019, 12, 1, 12, 0, 0)
            };
            addSuccess = tempController.AddTemp(testEntity);
            Assert.IsTrue(addSuccess);
            */
        }

        [Test]
        public void temp_controller_get_all_temps()
        {
            var tempController = new TempPlugin.TempController(_dbConnectionString);

            var addEntity = new TempPlugin.TempModel
            {
                Id = 1,
                Value = 1f,
                DateTime = new DateTime(2018, 12, 1, 12, 0, 0)
            };
            var addSuccess = tempController.AddTemp(addEntity);
            Assert.IsTrue(addSuccess);

            var addEntity2 = new TempPlugin.TempModel
            {
                Id = 2,
                Value = 2f,
                DateTime = new DateTime(2019, 12, 1, 12, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity2);
            Assert.IsTrue(addSuccess);

            var temps = tempController.GetTemps().ToArray();
            Assert.IsNotNull(temps);
            Assert.AreEqual(2, temps.Length);

            Assert.AreEqual(addEntity2.Id, temps[0].Id);
            Assert.AreEqual(addEntity2.DateTime, temps[0].DateTime);
            Assert.AreEqual(addEntity2.Value, temps[0].Value);
            Assert.AreEqual(addEntity.Id, temps[1].Id);
            Assert.AreEqual(addEntity.DateTime, temps[1].DateTime);
            Assert.AreEqual(addEntity.Value, temps[1].Value);
            
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity2.Id));
        }

        [Test]
        public void temp_controller_get_all_temps_of_date()
        {
            var tempController = new TempController(_dbConnectionString);

            var addEntity = new TempModel
            {
                Id = 1,
                Value = 1f,
                DateTime = new DateTime(2018, 12, 1, 12, 0, 0)
            };
            var addSuccess = tempController.AddTemp(addEntity);
            Assert.IsTrue(addSuccess);

            var addEntity2 = new TempModel
            {
                Id = 2,
                Value = 2f,
                DateTime = new DateTime(2019, 12, 1, 12, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity2);
            Assert.IsTrue(addSuccess);
            
            var addEntity3 = new TempModel
            {
                Id = 3,
                Value = 3f,
                DateTime = new DateTime(2019, 12, 1, 18, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity3);
            Assert.IsTrue(addSuccess);
            
            var temps = tempController.GetTempsByDate(new DateTime(2019,12,1)).ToArray();
            Assert.IsNotNull(temps);
            Assert.AreEqual(2, temps.Length);
            
            Assert.AreEqual(addEntity3.Id, temps[0].Id);
            Assert.AreEqual(addEntity3.DateTime, temps[0].DateTime);
            Assert.AreEqual(addEntity3.Value, temps[0].Value);
            Assert.AreEqual(addEntity2.Id, temps[1].Id);
            Assert.AreEqual(addEntity2.DateTime, temps[1].DateTime);
            Assert.AreEqual(addEntity2.Value, temps[1].Value);
            
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity2.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity3.Id));
        }

        [Test]
        public void temp_controller_get_paginated_list()
        {
            var tempController = new TempController(_dbConnectionString);

            var addEntity = new TempModel
            {
                Id = 1,
                Value = 1f,
                DateTime = new DateTime(2018, 12, 1, 12, 0, 0)
            };
            var addSuccess = tempController.AddTemp(addEntity);
            Assert.IsTrue(addSuccess);

            var addEntity2 = new TempModel
            {
                Id = 2,
                Value = 2f,
                DateTime = new DateTime(2019, 12, 1, 12, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity2);
            Assert.IsTrue(addSuccess);
            
            var addEntity3 = new TempModel
            {
                Id = 3,
                Value = 3f,
                DateTime = new DateTime(2019, 12, 1, 18, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity3);
            Assert.IsTrue(addSuccess);

            var getPaginatedList = tempController.GetTempsAsPaginatedList(1, 2);
            Assert.IsNotNull(getPaginatedList);
            Assert.AreEqual(getPaginatedList.PageSize, getPaginatedList.Items.ToArray().Length);
            Assert.AreEqual(3, getPaginatedList.Items.ToArray()[0].Id);
            Assert.AreEqual(2, getPaginatedList.Items.ToArray()[1].Id);

            getPaginatedList = tempController.GetTempsAsPaginatedList(2, 1); 
            Assert.IsNotNull(getPaginatedList);
            Assert.AreEqual(getPaginatedList.PageSize, getPaginatedList.Items.ToArray().Length);
            Assert.AreEqual(2, getPaginatedList.Items.ToArray()[0].Id);
            
            getPaginatedList = tempController.GetTempsAsPaginatedList(5, 1); 
            Assert.IsNotNull(getPaginatedList);
            Assert.AreEqual(0, getPaginatedList.Items.ToArray().Length);

            Assert.IsTrue(tempController.RemoveTemp((int) addEntity.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity2.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity3.Id));
        }
        
         [Test]
        public void temp_controller_get_paginated_list_of_date()
        {
            var tempController = new TempController(_dbConnectionString);

            var addEntity = new TempModel
            {
                Id = 1,
                Value = 1f,
                DateTime = new DateTime(2018, 12, 1, 12, 0, 0)
            };
            var addSuccess = tempController.AddTemp(addEntity);
            Assert.IsTrue(addSuccess);

            var addEntity2 = new TempModel
            {
                Id = 2,
                Value = 2f,
                DateTime = new DateTime(2019, 12, 1, 12, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity2);
            Assert.IsTrue(addSuccess);
            
            var addEntity3 = new TempModel
            {
                Id = 3,
                Value = 3f,
                DateTime = new DateTime(2019, 12, 1, 18, 0, 0)
            };
            addSuccess = tempController.AddTemp(addEntity3);
            Assert.IsTrue(addSuccess);

            var getPaginatedList = tempController.GetTempsByDateAsPaginatedList(new DateTime(2018, 12 ,1), 1, 2);
            Assert.IsNotNull(getPaginatedList);
            Assert.AreEqual(1, getPaginatedList.Items.ToArray().Length);
            Assert.AreEqual(1, getPaginatedList.Items.ToArray()[0].Id);

            Assert.IsTrue(tempController.RemoveTemp((int) addEntity.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity2.Id));
            Assert.IsTrue(tempController.RemoveTemp((int) addEntity3.Id));
        }
    }
}