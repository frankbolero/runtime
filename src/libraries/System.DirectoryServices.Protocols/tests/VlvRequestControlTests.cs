// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace System.DirectoryServices.Protocols.Tests
{
    [ConditionalClass(typeof(DirectoryServicesTestHelpers), nameof(DirectoryServicesTestHelpers.IsWindowsOrLibLdapIsInstalled))]
    public class VlvRequestControlTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var control = new VlvRequestControl();
            Assert.Equal(0, control.AfterCount);
            Assert.Equal(0, control.BeforeCount);
            Assert.True(control.IsCritical);
            Assert.Equal(0, control.Offset);
            Assert.Equal(0, control.EstimateCount);
            Assert.Empty(control.Target);
            Assert.Empty(control.ContextId);
            Assert.True(control.ServerSide);
            Assert.Equal("2.16.840.1.113730.3.4.9", control.Type);

#if NETFRAMEWORK
            var expected = new byte[] { 48, 132, 0, 0, 0, 18, 2, 1, 0, 2, 1, 0, 160, 132, 0, 0, 0, 6, 2, 1, 0, 2, 1, 0 };
#else
            var expected = new byte[] { 48, 14, 2, 1, 0, 2, 1, 0, 160, 6, 2, 1, 0, 2, 1, 0 };
#endif
            Assert.Equal(expected, control.GetValue());
        }

        public static IEnumerable<object[]> Ctor_BeforeCount_AfterCount_Offset_Data()
        {
#if NETFRAMEWORK
            yield return new object[] { 0, 0, 0, new byte[] { 48, 132, 0, 0, 0, 18, 2, 1, 0, 2, 1, 0, 160, 132, 0, 0, 0, 6, 2, 1, 0, 2, 1, 0 } };
            yield return new object[] { 10, 10, 10, new byte[] { 48, 132, 0, 0, 0, 18, 2, 1, 10, 2, 1, 10, 160, 132, 0, 0, 0, 6, 2, 1, 10, 2, 1, 0 } };
#else
            yield return new object[] { 0, 0, 0, new byte[] { 48, 14, 2, 1, 0, 2, 1, 0, 160, 6, 2, 1, 0, 2, 1, 0 } };
            yield return new object[] { 10, 10, 10, new byte[] { 48, 14, 2, 1, 10, 2, 1, 10, 160, 6, 2, 1, 10, 2, 1, 0 } };
#endif
        }

        [Theory]
        [MemberData(nameof(Ctor_BeforeCount_AfterCount_Offset_Data))]
        public void Ctor_BeforeCount_AfterCount_Offset(int beforeCount, int afterCount, int offset, byte[] expectedValue)
        {
            var control = new VlvRequestControl(beforeCount, afterCount, offset);
            Assert.Equal(afterCount, control.AfterCount);
            Assert.Equal(beforeCount, control.BeforeCount);
            Assert.True(control.IsCritical);
            Assert.Equal(offset, control.Offset);
            Assert.Equal(0, control.EstimateCount);
            Assert.Empty(control.Target);
            Assert.Empty(control.ContextId);
            Assert.True(control.ServerSide);
            Assert.Equal("2.16.840.1.113730.3.4.9", control.Type);

            Assert.Equal(expectedValue, control.GetValue());
        }

        public static IEnumerable<object[]> Ctor_BeforeCount_AfterCount_StringTarget_Data()
        {
#if NETFRAMEWORK
            yield return new object[] { 0, 0, null, new byte[0], new byte[] { 48, 132, 0, 0, 0, 18, 2, 1, 0, 2, 1, 0, 160, 132, 0, 0, 0, 6, 2, 1, 0, 2, 1, 0 } };
            yield return new object[] { 10, 10, "abc", new byte[] { 97, 98, 99 }, new byte[] { 48, 132, 0, 0, 0, 11, 2, 1, 10, 2, 1, 10, 129, 3, 97, 98, 99 } };
#else
            yield return new object[] { 0, 0, null, new byte[0], new byte[] { 48, 14, 2, 1, 0, 2, 1, 0, 160, 6, 2, 1, 0, 2, 1, 0 } };
            yield return new object[] { 10, 10, "abc", new byte[] { 97, 98, 99 }, new byte[] { 48, 11, 2, 1, 10, 2, 1, 10, 129, 3, 97, 98, 99 } };
#endif
        }

        [Theory]
        [MemberData(nameof(Ctor_BeforeCount_AfterCount_StringTarget_Data))]
        public void Ctor_BeforeCount_AfterCount_StringTarget(int beforeCount, int afterCount, string target, byte[] expectedTarget, byte[] expectedValue)
        {
            var control = new VlvRequestControl(beforeCount, afterCount, target);
            Assert.Equal(afterCount, control.AfterCount);
            Assert.Equal(beforeCount, control.BeforeCount);
            Assert.True(control.IsCritical);
            Assert.Equal(0, control.Offset);
            Assert.Equal(0, control.EstimateCount);
            Assert.NotSame(target, control.Target);
            Assert.Equal(expectedTarget ?? Array.Empty<byte>(), control.Target);
            Assert.Empty(control.ContextId);
            Assert.True(control.ServerSide);
            Assert.Equal("2.16.840.1.113730.3.4.9", control.Type);

            Assert.Equal(expectedValue, control.GetValue());
        }

        public static IEnumerable<object[]> Ctor_BeforeCount_AfterCount_ByteArrayTarget_Data()
        {
#if NETFRAMEWORK
            yield return new object[] { 0, 0, null, new byte[] { 48, 132, 0, 0, 0, 18, 2, 1, 0, 2, 1, 0, 160, 132, 0, 0, 0, 6, 2, 1, 0, 2, 1, 0 } };
            yield return new object[] { 10, 10, new byte[] { 1, 2, 3 }, new byte[] { 48, 132, 0, 0, 0, 11, 2, 1, 10, 2, 1, 10, 129, 3, 1, 2, 3 } };
#else
            yield return new object[] { 0, 0, null, new byte[] { 48, 14, 2, 1, 0, 2, 1, 0, 160, 6, 2, 1, 0, 2, 1, 0 } };
            yield return new object[] { 10, 10, new byte[] { 1, 2, 3 }, new byte[] { 48, 11, 2, 1, 10, 2, 1, 10, 129, 3, 1, 2, 3 } };
#endif
        }

        [Theory]
        [MemberData(nameof(Ctor_BeforeCount_AfterCount_ByteArrayTarget_Data))]
        public void Ctor_BeforeCount_AfterCount_ByteArrayTarget(int beforeCount, int afterCount, byte[] target, byte[] expectedValue)
        {
            var control = new VlvRequestControl(beforeCount, afterCount, target);
            Assert.Equal(afterCount, control.AfterCount);
            Assert.Equal(beforeCount, control.BeforeCount);
            Assert.True(control.IsCritical);
            Assert.Equal(0, control.Offset);
            Assert.Equal(0, control.EstimateCount);
            Assert.NotSame(target, control.Target);
            Assert.Equal(target ?? Array.Empty<byte>(), control.Target);
            Assert.Empty(control.ContextId);
            Assert.True(control.ServerSide);
            Assert.Equal("2.16.840.1.113730.3.4.9", control.Type);

            Assert.Equal(expectedValue, control.GetValue());
        }

        [Fact]
        public void Ctor_NegativeBeforeCount_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(-1, 0, "target"));
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(-1, 0, 0));
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(-1, 0, new byte[0]));
        }

        [Fact]
        public void Ctor_NegativeAfterCount_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(0, -1, "target"));
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(0, -1, 0));
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(0, -1, new byte[0]));
        }

        [Fact]
        public void Ctor_NegativeOffset_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("value", () => new VlvRequestControl(0, 0, -1));
        }

#if NET
        [Fact]
        public void Ctor_InvalidUtf8Target_ThrowsEncoderFallbackException()
        {
            AssertExtensions.Throws<EncoderFallbackException>(() => new VlvRequestControl(0, 0, "\uDD74\uD800"));
        }
#endif

        [Fact]
        public void EstimateCount_SetValid_GetReturnsExpected()
        {
            var control = new VlvRequestControl { EstimateCount = 10 };
            Assert.Equal(10, control.EstimateCount);
        }

        [Fact]
        public void EstimateCount_SetNegative_ThrowsArgumentException()
        {
            var control = new VlvRequestControl();
            AssertExtensions.Throws<ArgumentException>("value", () => control.EstimateCount = -1);
        }

        [Fact]
        public void ContextId_Set_GetReturnsExpected()
        {
            byte[] contextId = new byte[] { 1, 2, 3 };
            var control = new VlvRequestControl { ContextId = contextId };
            Assert.NotSame(contextId, control.ContextId);
            Assert.Equal(contextId, control.ContextId);

#if NETFRAMEWORK
            var expected = new byte[] { 48, 132, 0, 0, 0, 23, 2, 1, 0, 2, 1, 0, 160, 132, 0, 0, 0, 6, 2, 1, 0, 2, 1, 0, 4, 3, 1, 2, 3 };
#else
            var expected = new byte[] { 48, 19, 2, 1, 0, 2, 1, 0, 160, 6, 2, 1, 0, 2, 1, 0, 4, 3, 1, 2, 3 };
#endif
            Assert.Equal(expected, control.GetValue());
        }
    }
}
