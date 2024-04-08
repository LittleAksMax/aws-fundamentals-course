using Amazon.S3;
using Amazon.S3.Model;

var s3Client = new AmazonS3Client();

await using var fileStream = new FileStream("E:/AWS Learning/AWS Fundamentals/S3.Console/bravo_inventory.xlsx",
    FileMode.Open, FileAccess.Read);

var putObjectRequest = new PutObjectRequest
{
    BucketName = "davidr-awsfundamentalscourse",
    Key = "inventory.xlsx",
    ContentType = "application/x-msexcel",
    InputStream = fileStream
};

await s3Client.PutObjectAsync(putObjectRequest);

/*
var getObjectRequest = new GetObjectRequest
{
    BucketName = "davidr-awsfundamentalscourse",
    Key = "bravo_inventory.xlsx"
};

var getObjResponse = await s3Client.GetObjectAsync(getObjectRequest);
*/

//var delObjRequest = new DeleteObjectRequest
//{
//    BucketName = getObjResponse.BucketName,
//    Key = getObjResponse.Key
//};

//var deleteObjResponse = await s3Client.DeleteObjectAsync(delObjRequest);

// using var memoryStream = new MemoryStream();

// getObjResponse.ResponseStream.CopyTo(memoryStream);

// Console.WriteLine(Encoding.Default.GetString(memoryStream.ToArray()));

/** NOTES FOR API IMPLEMENTATION
 * S3Service would require 3 methods usually replacing Object with 'ExcelFile' or 'Image', etc.:
 * - CreateObject(IFormFile file): PutObjectResponse
 * You get the data in the controller as [FromForm(Name = "Data")] IFormFile file (so it is called Data in the request body)
 * file.ContentType for ContentType
 * file.OpenReadStream() for InputStream
 * MetaData {
 *     "x-amz-meta-originalname"
 *     "x-amz-meta-extension" = Path.GetExtension(file.FileName)
 * }
 * - GetObject(): GetObjectResponse
 * return File(getObjResponse.ResponseStream, response.Headers.ContentType)
 * - DeleteObject(): DeleteObjectResponse
 * The delete response has a HttpStatusCode that can be switched on to make the response easier.
 * This service can be injected into the controller responsible for managing the files
 * Usually AmazonS3Exception is thrown (but you can get more specialised).
 * 
 */

// REACTING TO CHANGES
