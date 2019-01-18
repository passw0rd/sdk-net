﻿namespace Passw0rd.Tests
{
    using System;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Security;
    using Passw0rd.Phe;
    using Passw0rd.Utils; 

    using Xunit;
    using Xunit.Abstractions;

    public class PheCryptoTests
    {
        private readonly ITestOutputHelper output;
        private byte[] serverPublic = Bytes.FromString("04464173c0589a4dd70760f0fd8ddccf99ec829098d194e9c925403a35245d44f2acf6784fe4d7a5eb76ba0d23227625e0f264051c8ed36fe9088f210faa160a45", StringEncoding.HEX);
        private byte[] serverPrivate = Bytes.FromString("403cde159ac7bafa8e04a88e3dcbaae06c3c2f46699c0a28344e5e54e3c460ca", StringEncoding.HEX);
        private byte[] clientPrivate = Bytes.FromString("f587c094be766cf7d33120717bdf7e448cfea9c7ea69d4cae49f145e1f967b6c", StringEncoding.HEX);
        private byte[] enrollmentResponse = Bytes.FromString("0a20fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d124104a623af89752c65f091ee3a1bad2101fa1fd07af69630ff1b4362d48f6209ec2d46583eefe98e92b5101eac9627da22cf70ef1f0ca5f5bcb9a2b2868ee746e5e71a410446bfd5f5087e274ff9047d0afdf78f41f7984338e0285ab009eda24c347e5708346c30dfff0581c5086d9d9e348a54a361f5f0ed3f0f8627fe52c8694e592c9622eb010a4104eba8f5f434ab5e9e1d29b01dd6c90a4921b01bc3c27f508f4c750c1a3156ee32e89e336f96f5883cd03441b03d5543c3d869a71b8ed8ae43c12fe03cd67aeefd124104df22dc8b1ce0d11fe23e2b7a5efaa7e1881cf7d0a66bf25ad5979bc8b0b33876c20fbdb2dcf90f4bcb168bbb1cd5ece1217cad813a4b4a9774503b4ffbf6b9e21a4104e6dcadc5be875aafeb95e97bfd52b02560d30be3d1ba12662e44a408310a900bc14e79a70912329e0e58a5db5e6b54f2d674751a90544c1b171cde64481fc77c2220f09dbd69f7886dbba4b5527b52479cde3de8b2737641727d5e2476846e26ec2d", StringEncoding.HEX);
        private byte[] password = Bytes.FromString("7061737377307264", StringEncoding.HEX);
        private byte[] enrollmentRecord = Bytes.FromString("0a20fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d1220fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d1a41041cc2fc1243a1e6af99c6099c2bde1c1bc866a6072975f87c6bfce11db719b09f4832ac49db8ea7dc811f3239fee5b531a530e9a9915eb7be7ac51decb3cf7753224104bf0d5ccbc453fb0149c1c9789511ec6f0a85e07a1f9e3943b7826f23f53dfe61aa040abc0e41686702690ea496344a528be4e862da1786482db29631e634068c", StringEncoding.HEX);
        private byte[] recordKey = Bytes.FromString("ffa2b491260f2b4ae5cf0849371fc521c3aa06a7f359bd1d30ad4b7de38ba316", StringEncoding.HEX);
        private byte[] verifyPasswordReq = Bytes.FromString("0a20fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d124104a623af89752c65f091ee3a1bad2101fa1fd07af69630ff1b4362d48f6209ec2d46583eefe98e92b5101eac9627da22cf70ef1f0ca5f5bcb9a2b2868ee746e5e7", StringEncoding.HEX);
        private byte[] verifyPasswordResp = Bytes.FromString("080112410446bfd5f5087e274ff9047d0afdf78f41f7984338e0285ab009eda24c347e5708346c30dfff0581c5086d9d9e348a54a361f5f0ed3f0f8627fe52c8694e592c961aeb010a4104684ab1bdffa84453ebd6a1ec23cc7b4ae2ce6ebbe5a21b16856fd3847dad4559a312525ba0ab53d24a41fa5192ccdc742767d61a04318cd7b5b332d6741287af12410461c82f541a04b37e40545b4756325e4de1e8ba542d97dda356016694d87ae4cc6b3f25844d6504c1316e5c5442ce098d04a103257ac15b9ff16d4994d2af59cd1a410401d217f6d2ca53a85562cf36d13cf17f4db5c2737d8027afdad2bbfc8dcf0d9986c06e144c7ac5032d9fac36815be395ad9f343c3229a78e8a5e9c806181230e2220f90282dccb78ac4f14487ac00df7c736eee55e6d63e53d4e0e7679a2e1b3c8a1", StringEncoding.HEX);
        private byte[] badPassword = Bytes.FromString("7040737377307264", StringEncoding.HEX);
        private byte[] verifyBadPasswordReq = Bytes.FromString("0a20fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d1241045950d1b2c56d8a77645fc784ddaa080066c20e19cbcc4805df27cfde4c88b744b01531b76d6be98f514870a2e4d2f7fa5139e20c7b517c7e8e56120f6dd0f6d3", StringEncoding.HEX);
        private byte[] verifyBadPasswordResp = Bytes.FromString("1241041a83e2221aa9796f5a5022c35f8bc764503c0cfb992bbd4eefb22bcd3186d280cf2783030316a538919abe2d697370a31bfba5133c2d679a22aed3327fc1da4722d0020a4104434fd3eb8d9df8ffc12667f09696ead6194ad7a197817bca53670852a4e32c0197dfeb50367622a88969d448daf8a1adf1416b884d2a6ae430820bd8bc36b9e3124104a773a99986143acf4698a2bcf93fa1ad02a8565f0231604d5f655ff70999ff55e4e5b6ef498d52342abf3b3ff72164017e52471abb06011392112058e36a14351a41049afb7fc9904b4fa45ab9993f91af369a6854b0f44d9048792be7327c2f9878172d0d64f9991fbcf6054bd463bf05e16b2e52d07671ad6e9f8c146605b21f61e3224104ea343565b7b2a6b8248e09ed2a3e4c32dbd7af391369be67387d6ccc4ab5baff7486ce95bf34cbaaec2a37ca380c33b10014450ffeec70dfde4a5eeaac5090cc2a2077e964767b16707315626307386c2a9d139dfa54de87a70e998124311d76a1743220f7dff81df7cc0582277944a89a75979766c89ba8e0ff5040fa85707c350521fe", StringEncoding.HEX);
        private byte[] token = Bytes.FromString("0a20fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d122080390531494470be0b296501586bfcd9e131c39e2decc753d4f25fefd2281eea", StringEncoding.HEX);
        private byte[] rotatedServerPub = Bytes.FromString("04c3b315ac3bbc101d7f71d31899fa44aecef0b1b879fab84c7f623d1113e6f7228b3399c246b345c6df0fa7af07cf39b558b13af502910d6c3b42d690468c2f1b", StringEncoding.HEX);
        private byte[] rotatedServerSk = Bytes.FromString("001e0d5c37a3627a53fed34b9f3a4236d3f5b6faa72696998a1239903a4bd12b", StringEncoding.HEX);
        private byte[] rotatedClientSk = Bytes.FromString("ceb6e27585f969f5d5c5bfb8bdc8337f369f381cc5e32efdc123ab74b06a441e", StringEncoding.HEX);
        private byte[] updatedRecord = Bytes.FromString("0a20fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d1220fc9e1d89fa8b15e391f62b3de357b0f56fe4dec54a008c7556c477bc9679f83d1a4104b1dba4fc25dbe850c14278188adad7f39eb8977db5b364d2fb851f2238bf03ab473cdae0c20de7d528ba8de5043c2eb3eed3d9d45b2e99290ef97af147692aa02241044971b0118442b7dd7fc3b0d098a3afb4c33c62768da00814224eeea77e9bf539fa0bc4279e2fcaff63aac3cbae33050c4d6626fdf04373c2bbbdc7bc609f3a1f", StringEncoding.HEX);

        public PheCryptoTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Should_ComputeIdenticalC0AndC1Values_When_TheSameKeyAndNonceArePassed()
        {
            var phe = new PheCrypto();

            var skS = phe.DecodeSecretKey(Bytes.FromString("I4ETKFzr3QmUu+Olhp1L2KvRgjfseO530R/A+aQ80Go=", StringEncoding.BASE64));
            var nS = Bytes.FromString("4g1N7hTxVWEHesoGZ5eTwbufdSnRtzxzkEQaBkXWsL4=", StringEncoding.BASE64);
            var c0 = Bytes.FromString("BI4mieAp/rdVctneZnhj0Ucu8Sc4LGMu2P5z9j49iXtN3AhDBWgIS1A4kfLI1ktcQAJACK4vwgutomtuWSoYllc=", StringEncoding.BASE64);
            var c1 = Bytes.FromString("BHENDFDcDsaWwpZLAWXDvEXlrEpIwr1p+OESiRCSemnk41WdfObVsvGPsYNFopaCJN762vP4MINb9HGzjmbM+aU=", StringEncoding.BASE64);

            var (c00, c11) = phe.ComputeC(skS, nS);

            Assert.Equal(c0, c00);
            Assert.Equal(c1, c11);
        }

        [Fact]
        public void Should_ComputeTheSameC0Value_For_SpecifiedListOfParameters()
        {
            var phe = new PheCrypto();

            var pwd = Bytes.FromString("passw0rd");
            var skC = phe.DecodeSecretKey(Bytes.FromString("gPdKsQRz9Vmc/DnbfxCHUioU6omEa0Sg7pncSHOhA7I=", StringEncoding.BASE64));
            var nC = Bytes.FromString("CTnPcIn2xcz+4/9jjLuFZNJiWLBiohnaGVTb3X1zPt0=", StringEncoding.BASE64);
            var c0 = Bytes.FromString("BKAH5Rww+a9+8lzO3oYE8zfz2Oxp3+Xrv2jp9EFGRlS7bWU5iYoWQHZqrkM+UYq2TYON3Gz8w3mzLSy3yS0XlJw=", StringEncoding.BASE64);
            var t0 = Bytes.FromString("BPGwrvLl3CAolVo1RIoNT7TqTC3T66eLykxa/Vw1gyof+3scfaiqTAAQXQ57q6zEkEeJHjNl4GBghQceI/UNl7c=", StringEncoding.BASE64);

            var c00 = phe.ComputeC0(skC, pwd, nC, t0);

            Assert.Equal(c0, c00);
        }
        /*
        [Fact]
        public void Should_ProveTheProofOfSuccess_For_SpecifiedListOfParameters()
        {
            var phe   = new PheCrypto();

            var skS   = phe.DecodeSecretKey(Bytes.FromString("I4ETKFzr3QmUu+Olhp1L2KvRgjfseO530R/A+aQ80Go=", StringEncoding.BASE64));
            var c0    = Bytes.FromString("BKAH5Rww+a9+8lzO3oYE8zfz2Oxp3+Xrv2jp9EFGRlS7bWU5iYoWQHZqrkM+UYq2TYON3Gz8w3mzLSy3yS0XlJw=", StringEncoding.BASE64);
            var c1    = Bytes.FromString("BOkgpxdavc+CYqmeKboBPhEjgXCEKbb5HxFeJ1+rVoOm006Jbu5a/Ol4rK5bVbkhzGAT+gUZaodUsnEuucfIbJc=", StringEncoding.BASE64);

            var proof = new ProofOfSuccessOld
            {
                Term1 = Bytes.FromString("BJkkIassffljoINliNRNcR7J/PmYcAl9PPUBeIT2I6oBy6wKb5gZsIOchWEN5pNfLVPH1V0BuhhlTi7MZeBlPOY=", StringEncoding.BASE64),
                Term2 = Bytes.FromString("BIiZfmmzuffwmJmibNnpws9D5SkgkPMBYxbgaWS1274kbJKC7WakUp1Mzk9BWvah0kDomJhqzyV7/8ZBX1rGX9I=", StringEncoding.BASE64),
                Term3 = Bytes.FromString("BCBcx9GsjQcoKI0pW4t49WWS/Z1bmg62KlCjAzB1IsKVDYQAT4213UuIMBthNnxSOVUUHikZCUw01mX5XKGQD5A=", StringEncoding.BASE64),
                BlindX = Bytes.FromString("KFiLnVVliWdm3fdYcuFK1sTiw1hvbKSesy4sGlYO8Rs=", StringEncoding.BASE64)
            };

            var skC = phe.DecodeSecretKey(Bytes.FromString("gPdKsQRz9Vmc/DnbfxCHUioU6omEa0Sg7pncSHOhA7I=", StringEncoding.BASE64));

            var nS  = Bytes.FromString("POKVRG0nmZc9062v41TNFngibsgMKzt/BY6lZ/5pcZg=", StringEncoding.BASE64);
            var pkS = phe.ExtractPublicKey(skS);

            var isValid = phe.ValidateProofOfSuccess(proof, pkS, nS, c0, c1);

            Assert.True(isValid);
        }

        [Fact]
        public void Should_ProveTheProofOfFail_When_SpecifiedListOfParametersArePassed()
        {
            var phe = new PheCrypto();

            var pkS = phe.DecodePublicKey(Bytes.FromString("BBVbS+bzzP5v7HxBs7p41HJT7mDuC8w5XcSsDMRmr/4fsH4mAFBkgcFrJ8kNqL1O5/BsTVp1eSn/vLlAZ6nMJM0=", StringEncoding.BASE64));
            var nS  = Bytes.FromString("bkVqMplyydZjQxo8R0EtODHEOqTfl02j8T5ZOa0tRnw=", StringEncoding.BASE64);
            var c0  = Bytes.FromString("BC5NNsFoaiMN1Flo/qPAzb0peVZSTJabpGR/ZW8y8t2iwqrJyQ7XiJfPzTFVGbDTbpF2NZOYJyoy8yWcu0ej/pk=", StringEncoding.BASE64);
            var c1  = Bytes.FromString("BA3VawoS0AHkkoqvdoAQY+Rny76K5qJGBXI6HPYpar9v1VQA4PXoHW7uWECW8ulljYMtP06696JcNmQTsjYmDdk=", StringEncoding.BASE64);

            var proof = new ProofOfFailOld
            {
                Term1  = Bytes.FromString("BEY086yK/21rcM/L1o1VlgFbG543aHd5wsSz149MAqsE9PjKOkBlLgo4L8erUZkyW9rnJlVy2OlppjJ5ti17JXs=", StringEncoding.BASE64),
                Term2  = Bytes.FromString("BGB1gW1fJAJZKIicx5BBoGjCvsA29FONmVZ9KJQYB1pQoTRvz4LuF1m6BB7e1HtT58piuk8ZxHFqF4gmEDbTUiU=", StringEncoding.BASE64),
                Term3  = Bytes.FromString("BPgnI6MoiihA1C/VdfvFN1f4nEd9Cvh5Mp4fRppYsOXjUBuB70jNlLq02DHLqlkcASEsL0wORH7LZbTqUdaEKgY=", StringEncoding.BASE64),
                Term4  = Bytes.FromString("BFlUCX9E6QOpxxJOWuGhPujJOuJdVKFaU1C8aSyiHFSgcYB5PCp77Ir4fKPLQmHkMpAN65DokctO08d41E8a1Uk=", StringEncoding.BASE64),
                BlindA = Bytes.FromString("QAucC4Dzg9/qcJsDoDopkRXsja1uAsOCQw0qKuEaEn8=", StringEncoding.BASE64),
                BlindB = Bytes.FromString("Ub1/iklGLDXe+DwoviQ3tSiWd9hWTUpBJfWKhl9CSok=", StringEncoding.BASE64)
            };

            var isValid = phe.ValidateProofOfFail(proof, pkS, nS, c0, c1);

            Assert.True(isValid);
        }

        [Fact]
        public void Should_DecryptM_When_SpecifiedListOfParametersArePassed()
        {
            var phe = new PheCrypto();

            var pwd = Bytes.FromString("passw0rd");
            var skC = phe.DecodeSecretKey(Bytes.FromString("Ty+3kM7kPFQjZu1NKyJjHqdUnDl9/n7mlHu7VJMyLRw=", StringEncoding.BASE64));
            var nC  = Bytes.FromString("9Gr0Jrg+vux+xBOLpGLXemB7uuwq7xkIa4JVgAJG5Yw=", StringEncoding.BASE64);
            var c1  = Bytes.FromString("BMkSH2xpOLox46HQf0oq8vqtYApMezo7K83xCMDgsUzgpI75esc6KJTRrqf7Nq5+y2LiGwyfJ/X8wZ7IXywy+gg=", StringEncoding.BASE64);
            var t1  = Bytes.FromString("BPGSx7b84QaTFJoXJcQ70qPuppbGkh8udqSIZJ7R8AE6Br0CBvPw69exsa3jxeHmMN17vY9l9wXdZcQt7FST2fc=", StringEncoding.BASE64);
            var m   = Bytes.FromString("XslZXIrsVj1XYVF74gWfi0Lo4frLsDObirNl5mFwYeA=", StringEncoding.BASE64);

            var mm  = phe.DecryptM(skC, pwd, nC, t1, c1);

            Assert.Equal(m, mm);
        }

        [Fact]
        public void Should_RotateTheSameSecretKey_When_OldSecretKeyAndUpdateTokenAreGiven()
        {
            var a = Bytes.FromString("T20buheJjFOg+rsxP5ADIS7G3htdY/MUt9VozMOgEfA=", StringEncoding.BASE64);
            var b = Bytes.FromString("UbXPXPtmKuudthZXXjJTE9AxBEgZB7mTFD+TGViCgHU=", StringEncoding.BASE64);
            var skC = Bytes.FromString("YOCs/LOx6hll3nUBC29xpNJuLXofpKaBUNHPDBMA7JI=", StringEncoding.BASE64);
            var skC1 = Bytes.FromString("Zd+YJUvpXKQIjMaeZiad4vFOoU+mH2Qldx/yqwmGg2I=", StringEncoding.BASE64);
            // var pkS = Bytes.FromString("BBqqpApF8EsvQtLQlcR1sBon9RbKDcrsNypYDGatbx5JxvdQfGaszDwen01xQVWxL0UvrLfmzTBJHpL+q5+kyWw=", StringEncoding.BASE64);

            var phe = new PheCrypto();
            var pheSkC = phe.DecodeSecretKey(skC);

            var pheSkC1 = phe.RotateSecretKey(pheSkC, a, b);

            Assert.Equal(skC1, pheSkC1.Encode());
        }

        [Fact]
        public void Should_RotateTheSamePublicKey_When_OldPublicKeyAndUpdateTokenAreGiven()
        {
            var a = Bytes.FromString("T20buheJjFOg+rsxP5ADIS7G3htdY/MUt9VozMOgEfA=", StringEncoding.BASE64);
            var b = Bytes.FromString("UbXPXPtmKuudthZXXjJTE9AxBEgZB7mTFD+TGViCgHU=", StringEncoding.BASE64);
            var pkS = Bytes.FromString("BBqqpApF8EsvQtLQlcR1sBon9RbKDcrsNypYDGatbx5JxvdQfGaszDwen01xQVWxL0UvrLfmzTBJHpL+q5+kyWw=", StringEncoding.BASE64);
            var pkS1 = Bytes.FromString("BMiu/KcLEom9PwAeEeN9gYJZ45kdlYdo1bYPsd8YjWvRVgqJY2MzJlu2OR1d7ynxZvsdXbVY68pxG/oK3k+3xX0=", StringEncoding.BASE64);

            var phe = new PheCrypto();
            var phePkC = phe.DecodePublicKey(pkS);

            var phePkC1 = phe.RotatePublicKey(phePkC, a, b);

            Assert.Equal(pkS1, phePkC1.Encode());
        }*/
        [Fact]
        public void TestEncrypt(){
            var phe = new PheCrypto();
            var rng = new SecureRandom();
            var key = new byte[phe.SymKeyLen()];
            rng.NextBytes(key);

            var plainText = new byte[365];
            rng.NextBytes(plainText);
            var cipherText = phe.Encrypt(plainText, key);
            var decyptedText = phe.Decrypt(cipherText, key);
            Assert.Equal(decyptedText, plainText);
        }

        [Fact]
        public void TestEncrypt_empty(){
            var phe = new PheCrypto();
            var rng = new SecureRandom();
            var key = new byte[phe.SymKeyLen()];
            rng.NextBytes(key);

            var plainText = new byte[0];
            var cipherText = phe.Encrypt(plainText, key);
            var decyptedText = phe.Decrypt(cipherText, key);
            Assert.Equal(decyptedText, plainText);
        }

        [Fact]
        public void TestEncrypt_badKey(){
            var phe = new PheCrypto();
            var rng = new SecureRandom();
            var key = new byte[phe.SymKeyLen()];
            rng.NextBytes(key);

            var plainText = new byte[365];
            rng.NextBytes(plainText);
           
            var cipherText = phe.Encrypt(plainText, key);

            key[0]++;

            var ex = Record.Exception(() => { phe.Decrypt(cipherText, key); });

            Assert.NotNull(ex);
            Assert.IsType<InvalidCipherTextException>(ex);
        }
        [Fact]
        public void TestDecrypt_badLength(){
            var phe = new PheCrypto();
            var rng = new SecureRandom();
            var key = new byte[phe.SymKeyLen()];
            rng.NextBytes(key);

            var cipherText = new byte[phe.SymSaltLen() + 15];
            rng.NextBytes(cipherText);
           
            var ex = Record.Exception(() => {phe.Decrypt(cipherText, key); });

            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void TestEncryptVector(){
            var rnd = new byte[]{
                0x2b, 0x1a, 0x49, 0xe2, 0x6c, 0xcc, 0x33, 0xfe,
                0x5e, 0x7d, 0x0e, 0x57, 0x3b, 0xc4, 0x02, 0xf0,
                0x04, 0xa0, 0x1c, 0x60, 0x35, 0xaf, 0x42, 0x16,
                0xcb, 0xd0, 0x1f, 0x1a, 0x98, 0x24, 0x7a, 0xaa,
            };

            var key = new byte[]{
                0x87, 0xeb, 0x2b, 0xc9, 0x09, 0xac, 0x86, 0x9a,
                0xdc, 0xb2, 0x17, 0x72, 0x2f, 0x3f, 0x56, 0xa6,
                0xf7, 0x0f, 0xb7, 0x47, 0x3b, 0x1b, 0x6b, 0x36,
                0xf0, 0xae, 0x0a, 0x14, 0x5b, 0x45, 0xae, 0xe2,
            };

            var plainText = new byte[]{
                0x05, 0xa1, 0x06, 0x74, 0xa5, 0xba, 0xd0, 0x38,
                0x50, 0x7b, 0x2d, 0x9f, 0x80, 0x06, 0x45, 0x4b,
                0x0f, 0xbe, 0xf0, 0xd4, 0x0f, 0x62, 0x1b, 0x3c,
                0x35, 0x16, 0xef, 0xdd, 0x70, 0xd1, 0xef, 0x1d,
                0x3a, 0x6b, 0x8d, 0x50, 0xbe, 0xdb, 0x25, 0x57,
                0x3c, 0x26, 0x86, 0x43, 0x86, 0xa1, 0x39, 0x69,
                0xf7, 0xe9, 0x40, 0x00, 0xf0, 0x02, 0xd0, 0x0f,
                0xae, 0x86, 0x84, 0x37, 0x77, 0x0d, 0x9a, 0xfa,
            };

            var expectedCipherText = new byte[]{
                0x2b, 0x1a, 0x49, 0xe2, 0x6c, 0xcc, 0x33, 0xfe,
                0x5e, 0x7d, 0x0e, 0x57, 0x3b, 0xc4, 0x02, 0xf0,
                0x04, 0xa0, 0x1c, 0x60, 0x35, 0xaf, 0x42, 0x16,
                0xcb, 0xd0, 0x1f, 0x1a, 0x98, 0x24, 0x7a, 0xaa,
                0x61, 0x95, 0x05, 0xda, 0x9c, 0xbf, 0x32, 0x5b,
                0x79, 0x2a, 0x31, 0xce, 0x87, 0x71, 0x6e, 0x89,
                0xc0, 0x0c, 0xe9, 0x32, 0x14, 0xb1, 0x5c, 0x59,
                0x6b, 0x30, 0xe6, 0xe5, 0x1a, 0xed, 0x8a, 0x3c,
                0xdd, 0x83, 0x1e, 0xbf, 0x0e, 0xa7, 0x7f, 0x59,
                0x4d, 0xae, 0xed, 0x9c, 0xa0, 0xb8, 0xe6, 0x28,
                0x0c, 0x73, 0x60, 0xbc, 0x8c, 0x0f, 0xd7, 0xb9,
                0x2d, 0x09, 0x40, 0x0c, 0x8d, 0x63, 0x36, 0x19,
                0x32, 0x04, 0xac, 0xd4, 0x45, 0xa0, 0xa4, 0x5e,
                0xab, 0x08, 0x2c, 0xb1, 0xa7, 0x36, 0x04, 0xf4,
            };

            var phe = new PheCrypto();
            var rng = new SecureRandom();

            var cipherText = phe.Encrypt(plainText, key);
            Assert.Equal(cipherText, expectedCipherText);

            var decyptedText = phe.Decrypt(cipherText, key);
            Assert.Equal(plainText, decyptedText);
        }

        [Fact]
        public void TestHashZVector1(){
            
        }

        [Fact]
        public void TestHashZVector2()
        {
        }

        [Fact]
        public void PointToBytes()
        {
        }

        [Fact]
        public void HashZ()
        {
        }

        [Fact]
        public void DataToHash()
        {
        }

        [Fact]
        public void hc0()
        { 
        }

        [Fact]
        public void hc1()
        {
        }


        [Fact]
        public void hs0()
        {
        }

        [Fact]
        public void hs1()
        {
        }


    }
}
