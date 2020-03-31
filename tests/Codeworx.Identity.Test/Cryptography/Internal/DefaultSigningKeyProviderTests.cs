﻿using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.OpenId.Model;
using Xunit;

namespace Codeworx.Identity.Test.Cryptography.Internal
{
    public class DefaultSigningKeyProviderTests
    {
        [Fact]
        public void GetKeyParameter_ReturnsECA()
        {
            var expectedKeyId = string.Empty;
            var expectedKeyType = KeyType.EllipticCurve;
            var expectedKeyUse = KeyUse.Signature;
            var expectedCurveType = CurveType.P384;
            
            var instance = new DefaultSigningKeyProvider();

            var actual = instance.GetKeyParameter();

            Assert.IsType(typeof(EllipticKeyParameter), actual);
            Assert.Equal(expectedKeyId, actual.KeyId);
            Assert.Equal(expectedKeyType, actual.KeyType);
            Assert.Equal(expectedKeyUse, actual.KeyUse);
            var ellipticKeyParameter = actual as EllipticKeyParameter;
            Assert.NotNull(ellipticKeyParameter);
            Assert.Equal(expectedCurveType, ellipticKeyParameter.Curve);
        }
    }
}