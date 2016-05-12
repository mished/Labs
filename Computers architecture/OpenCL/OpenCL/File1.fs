    module Mished.Lab6

    open FSCL
    open FSCL.Compiler
    open FSCL.Language
    open FSCL.Runtime
    open System
    open System.Diagnostics
    open System.Drawing
    open System.Drawing.Imaging

    let LoadImage(source: Bitmap) =
        let height = source.Height
        let width = source.Width

        Array2D.init width height (fun x y ->
            let pix = source.GetPixel(x, y)
            uchar4(pix.A, pix.R, pix.G, pix.B))


    let StoreImage(source: uchar4[,]) =
        let height = source.GetLength 1
        let width = source.GetLength 0
        let image = new Bitmap(width, height)

        for i = 0 to width - 1 do
            for j = 0 to height - 1 do
                let pix = source.[i, j]
                let a = pix.x |> int
                let r = pix.y |> int
                let g = pix.z |> int
                let b = pix.w |> int
                image.SetPixel(i, j, Color.FromArgb(a, r, g, b))
        
        image


    [<ReflectedDefinition>]
    let SobelFilter2D(inputImage: uchar4[,], outputImage: uchar4[,], wi: WorkItemInfo) =
        let x = wi.GlobalID(0)
        let y = wi.GlobalID(1)

        let width = wi.GlobalSize(0)
        let height = wi.GlobalSize(1)

        let mutable Gx = float4(0.0f)
        let mutable Gy = Gx

        // Read each texel component and calculate the filtered value using neighbouring texel components 
        let i00 = (inputImage.[y, x]).ToFloat4()
        let i10 = (inputImage.[y, x + 1]).ToFloat4()
        let i20 = (inputImage.[y, x + 2]).ToFloat4()
        let i01 = (inputImage.[y + 1, x]).ToFloat4()
        let i11 = (inputImage.[y + 1, x + 1]).ToFloat4()
        let i21 = (inputImage.[y + 1, x + 2]).ToFloat4()
        let i02 = (inputImage.[y + 2, x]).ToFloat4()
        let i12 = (inputImage.[y + 2, x + 1]).ToFloat4()
        let i22 = (inputImage.[y + 2, x + 2]).ToFloat4()

        Gx <- i00 + float4(2.0f) * i10 + i20 - i02  - float4(2.0f) * i12 - i22
        Gy <- i00 - i20  + float4(2.0f) * i01 - float4(2.0f) * i21 + i02 - i22

        outputImage.[y, x] <- (float4.hypot(Gx, Gy)/float4(2.0f)).ToUChar4()
        Console.WriteLine((float4.hypot(Gx, Gy)/float4(2.0f)).ToUChar4().xyzw)
        outputImage


//    [<EntryPoint>]
//    let main argv =
//    
//        let image = new Bitmap("..\\..\\..\\img\\IMAGE.bmp")
//        let width = image.Width
//        let height = image.Height
//        let gsImage = image |> LoadImage
//        let resData : uchar4[,] = Array2D.zeroCreate image.Width image.Height
//
//        let sw = new Stopwatch()
//        sw.Start()
//        let ws = WorkSize([| (((width - 1) / 16) + 1) * 16 |> int64; (((height - 1) / 16) + 1) * 16 |> int64 |])
//        let res =
//            <@
//                SobelFilter2D(gsImage, resData, ws)
//            @>.Run()
//
//        sw.Stop()
//        printfn "%A" sw.ElapsedMilliseconds
//        let resBitmap = resData |> StoreImage
//        resBitmap.Save("SAVED.jpg")
//
//        0