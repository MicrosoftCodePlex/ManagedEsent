﻿//-----------------------------------------------------------------------
// <copyright file="ToStringTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

namespace InteropApiTests
{
    using System;
    using Microsoft.Isam.Esent.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Testing the ToString methods of the basic types.
    /// </summary>
    [TestClass]
    public class ToStringTests
    {
        /// <summary>
        /// Test JET_INSTANCE.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_INSTANCE.ToString()")]
        public void JetInstanceToString()
        {
            var instance = new JET_INSTANCE() { Value = (IntPtr)0x123ABC };
            Assert.AreEqual("JET_INSTANCE(0x123abc)", instance.ToString());
        }

        /// <summary>
        /// Test JET_SESID.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_SESID.ToString()")]
        public void JetSesidToString()
        {
            var sesid = new JET_SESID() { Value = (IntPtr)0x123ABC };
            Assert.AreEqual("JET_SESID(0x123abc)", sesid.ToString());
        }

        /// <summary>
        /// Test JET_DBID.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_DBID.ToString()")]
        public void JetDbidToString()
        {
            var dbid = new JET_DBID() { Value = 23 };
            Assert.AreEqual("JET_DBID(23)", dbid.ToString());
        }

        /// <summary>
        /// Test JET_TABLEID.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_TABLEID.ToString()")]
        public void JetTableidToString()
        {
            var tableid = new JET_TABLEID() { Value = (IntPtr)0x123ABC };
            Assert.AreEqual("JET_TABLEID(0x123abc)", tableid.ToString());
        }

        /// <summary>
        /// Test JET_COLUMNID.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_COLUMNID.ToString()")]
        public void JetColumnidToString()
        {
            var columnid = new JET_COLUMNID() { Value = 0x12EC };
            Assert.AreEqual("JET_COLUMNID(0x12ec)", columnid.ToString());
        }

        /// <summary>
        /// Test JET_OSSNAPID.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_OSSNAPID.ToString()")]
        public void JetOsSnapidToString()
        {
            var ossnapid = new JET_OSSNAPID { Value = (IntPtr)0x123ABC };
            Assert.AreEqual("JET_OSSNAPID(0x123abc)", ossnapid.ToString());
        }

        /// <summary>
        /// Test JET_HANDLE.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_HANDLE.ToString()")]
        public void JetHandleToString()
        {
            var handle = new JET_HANDLE { Value = (IntPtr)0x123ABC };
            Assert.AreEqual("JET_HANDLE(0x123abc)", handle.ToString());
        }

        /// <summary>
        /// Test JET_LS.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_LS.ToString()")]
        public void JetLsToString()
        {
            var handle = new JET_LS { Value = (IntPtr)0x123ABC };
            Assert.AreEqual("JET_LS(0x123abc)", handle.ToString());
        }

        /// <summary>
        /// Test JET_INDEXID.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_INDEXID.ToString()")]
        public void JetIndexIdToString()
        {
            var indexid = new JET_INDEXID { IndexId1 = (IntPtr)0x1, IndexId2 = 0x2, IndexId3 = 0x3 };
            Assert.AreEqual("JET_INDEXID(0x1:0x2:0x3)", indexid.ToString());
        }

        /// <summary>
        /// Test JET_LOGTIME.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_LOGTIME.ToString()")]
        public void JetLogtimeToString()
        {
            var logtime = new JET_LOGTIME(new DateTime(2010, 5, 31, 4, 44, 17, DateTimeKind.Utc));
            Assert.AreEqual("JET_LOGTIME(17:44:4:31:5:110:0x80:0x0)", logtime.ToString());
        }

        /// <summary>
        /// Test JET_BKLOGTIME.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_BKLOGTIME.ToString()")]
        public void JetBklogtimeToString()
        {
            var bklogtime = new JET_BKLOGTIME(new DateTime(2010, 5, 31, 4, 44, 17, DateTimeKind.Utc), true);
            Assert.AreEqual("JET_BKLOGTIME(17:44:4:31:5:110:0x80:0x80)", bklogtime.ToString());
        }

        /// <summary>
        /// Test JET_SIGNATURE.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_SIGNATURE.ToString()")]
        public void JetSignatureToString()
        {
            var t = new DateTime(2010, 5, 31, 4, 44, 17, DateTimeKind.Utc);
            var signature = new JET_SIGNATURE(99, t, "COMPUTER");
            Assert.AreEqual("JET_SIGNATURE(99:05/31/2010 04:44:17:COMPUTER)", signature.ToString());
        }

        /// <summary>
        /// Test JET_LGPOS.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_LGPOS.ToString()")]
        public void JetLgposToString()
        {
            var lgpos = new JET_LGPOS { lGeneration = 1, isec = 0x1F, ib = 3 };
            Assert.AreEqual("JET_LGPOS(0x1,1F,3)", lgpos.ToString());
        }

        /// <summary>
        /// Test JET_BKINFO.ToString().
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Description("Test JET_BKINFO.ToString()")]
        public void JetBkinfoToString()
        {
            var bklogtime = new JET_BKLOGTIME(new DateTime(2010, 5, 31, 4, 44, 17, DateTimeKind.Utc), true);
            var lgpos = new JET_LGPOS { lGeneration = 1, isec = 2, ib = 3 };
            var bkinfo = new JET_BKINFO { bklogtimeMark = bklogtime, genHigh = 57, genLow = 36, lgposMark = lgpos };
            Assert.AreEqual("JET_BKINFO(36-57:JET_LGPOS(0x1,2,3):JET_BKLOGTIME(17:44:4:31:5:110:0x80:0x80))", bkinfo.ToString());
        }
    }
}