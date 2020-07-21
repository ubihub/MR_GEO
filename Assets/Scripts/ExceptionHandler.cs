using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGC.Annotation.Basic
{
    public class ExceptionHandler
    {
        private ExceptionHandler()
        {

        }

        public static ExceptionHandler Instance { get { return Nested.instance; } }

        private class Nested
        {

            static Nested()
            {

            }

            internal static readonly ExceptionHandler instance = new ExceptionHandler();
        }

        public void GetException(Exception ex)
        {
            Debug.LogError(ex.StackTrace);

        }
    }
}