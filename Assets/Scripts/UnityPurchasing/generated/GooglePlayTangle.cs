// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("77VGa4izTw929aOvgUk/n+gu7ZUUpiUGFCkiLQ6ibKLTKSUlJSEkJ3L0ulHNtfxGcVqvZcyFCc+agt83qwWjhGadTN2Lth3SgX+vBg5guISU+//0LIUDDQbJE5E4VE6hRaV3OGt0mjTAjslcF+HTr+BlL3Fg0g+0DPg2pAWA1NOZ0hLFXG7qgihejBq28AxpMR23X8ZPIdRhzEjqGAoxmqYlKyQUpiUuJqYlJSSUXRPV8B+tfUh7ZdqMddCf5H2UnD2HID9uUCUtSwgLUd9REGDdKAUOFD7/J8qzKxYSxUJZNnDc8yddZWP2h+lOz2qsxOGi3ToBs6q2PoRZFOOCHjxj1j8mt2qJHSuDEGXfbAIobrBHCn8Gt69kMaKa9C+fJSYnJSQl");
        private static int[] order = new int[] { 13,13,3,3,11,7,7,12,13,12,11,11,13,13,14 };
        private static int key = 36;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
