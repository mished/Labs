open FSCL
open FSCL.Compiler
open FSCL.Language
open FSCL.Runtime
open System
open System.Diagnostics
open System.Drawing
open System.Drawing.Imaging

let Grayscale(image: Bitmap) =
    let width = image.Width
    let height = image.Height
    let rect = new Rectangle(0, 0, width, height)

    let data = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat)
    let ptr = data.Scan0
    let stride = data.Stride
    let bytes = stride * data.Height
    let values : byte[] = Array.zeroCreate bytes

    System.Runtime.InteropServices.Marshal.Copy(ptr, values, 0, bytes)
    image.UnlockBits(data)
    let pixelSize = stride / data.Width

    Array2D.init width height (fun x y ->
        let pos = stride * y + x * pixelSize
        let b = values.[pos] |> float
        let g = values.[pos + 1] |> float
        let r = values.[pos + 2] |> float
        r * 0.2126 + g * 0.7152 + b * 0.0722 |> byte)


let StoreImage(source: byte[,]) =
    let height = source.GetLength 0
    let width = source.GetLength 1
    let rect = new Rectangle(0, 0, width, height)
    let image = new Bitmap(width, height)

    let data = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat)
    let ptr = data.Scan0
    let stride = data.Stride
    let bytes = stride * data.Height
    let values : byte[] = Array.zeroCreate source.Length
    Buffer.BlockCopy(source, 0, values, 0, source.Length)

    let rgba = values |> Array.collect(fun x -> [| x; x; x; 255uy |])

    System.Runtime.InteropServices.Marshal.Copy(rgba, 0, ptr, rgba.Length)
    image.UnlockBits(data)
    image


let SobelFilter(inputImage: byte[,], x: int, y: int) =
    let height = inputImage.GetLength 0
    let width = inputImage.GetLength 1

    if (y = 0 || y = height - 1 || x = 0 || x = width - 1) then
        inputImage.[y, x]
    else
        let a = inputImage.[y - 1, x - 1]
        let b = inputImage.[y - 1, x]
        let c = inputImage.[y - 1, x + 1]
        let d = inputImage.[y, x - 1]
        let e = inputImage.[y, x]
        let f = inputImage.[y, x + 1]
        let g = inputImage.[y + 1, x - 1]
        let h = inputImage.[y + 1, x]
        let i = inputImage.[y + 1, x + 1]

        let t1 = a - i
        let t2 = c - g
        let Hh = 2uy * (d - f) + t1 - t2 |> float
        let Hv = 2uy * (b - h) + t1 + t2 |> float

        sqrt(Hh ** 2.0 + Hv ** 2.0) * 256.0 / 1140.0 |> byte


[<EntryPoint>]
let main argv =
    
    let image = new Bitmap("..\\..\\..\\img\\IMAGE.bmp")
    let gsImage = image |> Grayscale
    let resData : byte[,] = Array2D.zeroCreate image.Width image.Height

    let sw = new Stopwatch()
    sw.Start()

    let res =
        <@
            gsImage
            |> Array2D.mapi(fun y x _ -> SobelFilter(gsImage, x, y))
        @>.Run()

    sw.Stop()
    printfn "%A" sw.ElapsedMilliseconds
    let resBitmap = StoreImage(res)
    resBitmap.Save("SAVED.bmp")

    0