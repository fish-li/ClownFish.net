global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.Specialized;
global using System.Data;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Dynamic;
global using System.IO;
global using System.IO.Compression;
global using System.Linq;
global using System.Net;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;
global using System.Security.Cryptography;
global using System.Security.Cryptography.X509Certificates;
global using System.Security.Principal;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Xml.Serialization;

global using ClownFish.Base;
global using ClownFish.Base.Exceptions;
global using ClownFish.Base.Reflection;
global using ClownFish.Http.Utils;
global using ClownFish.WebClient;
global using ClownFish.Base.Xml;
global using ClownFish.Data;
global using ClownFish.Data.Internals;
global using ClownFish.Http.Pipleline;
global using ClownFish.Log;
global using ClownFish.Log.Attributes;
global using ClownFish.Log.Configuration;
global using ClownFish.Log.Logging;
global using ClownFish.Log.Models;

global using Newtonsoft.Json;


#if NET6_0_OR_GREATER
global using ClownFish.MQ.Pipeline;
#endif
