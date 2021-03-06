﻿/*
 * Copyright (C) 2015-2019 Virgil Security Inc.
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     (1) Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *
 *     (2) Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in
 *     the documentation and/or other materials provided with the
 *     distribution.
 *
 *     (3) Neither the name of the copyright holder nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ''AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
 * IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *
 * Lead Maintainer: Virgil Security Inc. <support@virgilsecurity.com>
*/

namespace Virgil.PureKit.Utils
{
    using System;

    internal static class Validation
    {
        public static void NotNull(object obj, string message = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message, nameof(obj));
            }
        }

        public static void NotNullOrWhiteSpace(string obj, string message = null)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentException(message, nameof(obj));
            }
        }

        public static void NotNullOrEmptyByteArray(byte[] obj, string message = null)
        {
            if (obj == null || obj.Length == 0)
            {
                throw new ArgumentException(message, nameof(obj));
            }
        }

        public static void NotNullOrEmptyByteArray(byte[][] byteArrays, string message = null)
        {
            if (byteArrays == null || byteArrays.Length == 0)
            {
                throw new ArgumentException(message, nameof(byteArrays));
            }

            foreach (var byteArray in byteArrays)
            {
                if (byteArray == null || byteArray.Length == 0)
                {
                    throw new ArgumentException(message, nameof(byteArrays));
                }
            }
        }

        public static void MoreThanZero(uint obj, string message = null)
        {
            if (obj <= 0)
            {
                throw new ArgumentException(message, nameof(obj));
            }
        }
    }
}