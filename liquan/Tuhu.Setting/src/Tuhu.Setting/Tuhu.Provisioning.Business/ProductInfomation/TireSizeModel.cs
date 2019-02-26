using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.Business.ProductInfomation
{
    public class TireSizeModel : BaseModel
    {
        public TireSizeModel() : base() { } 
        public TireSizeModel(string width, string aspectRatio, string rim)
            : base()
        {
            this.Width = width;
            this.AspectRatio = aspectRatio;
            this.Rim = rim;
        }

        /// <summary>胎宽(CP_Tire_Width)</summary>
        public string Width { get; set; }
        /// <summary>扁平比(CP_Tire_AspectRatio)</summary>
        public string AspectRatio { get; set; }
        /// <summary>轮毂尺寸(CP_Tire_Rim)</summary>
        public string Rim { get; set; }
         
        public override bool Equals(object obj)
        {
            var that = obj as TireSizeModel;

            if (that == null)
                return false;

            return this.ToString() == that.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static TireSizeModel Parse(string size)
        {
            if (string.IsNullOrWhiteSpace(size))
                return null;

            var array = size.Split(new char[] { '/', 'R' });

            if (array.Length != 3)
                return null;

            return new TireSizeModel(array[0], array[1], array[2]);
        }

        public static bool operator ==(TireSizeModel objA, TireSizeModel objB)
        {
            return object.Equals(objA, objB);
        }
        public static bool operator !=(TireSizeModel objA, TireSizeModel objB)
        {
            return !object.Equals(objA, objB);
        }

        public override string ToString()
        {
            return string.Concat(this.Width, "/", this.AspectRatio, "R", this.Rim);
        }
    }
}
