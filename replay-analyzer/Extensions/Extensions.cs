using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SingleStoreConnector;
namespace FortniteReplayAnalyzer.Extensions
{
  public static class Extensions
  {
    public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
    {
      foreach (var value in list)
      {
        await func(value);
      }
    }
    public static List<List<T>> partition<T>(this List<T> values, int chunkSize)
    {
      return values.Select((x, i) => new { Index = i, Value = x })
          .GroupBy(x => x.Index / chunkSize)
          .Select(x => x.Select(v => v.Value).ToList())
          .ToList();
    }
    public static void ToCSV(this DataTable dtDataTable, string strFilePath)
    {
      StreamWriter sw = new StreamWriter(strFilePath, false);
      //headers    
      for (int i = 0; i < dtDataTable.Columns.Count; i++)
      {
        sw.Write(dtDataTable.Columns[i]);
        if (i < dtDataTable.Columns.Count - 1)
        {
          sw.Write(",");
        }
      }
      sw.Write(sw.NewLine);
      foreach (DataRow dr in dtDataTable.Rows)
      {
        for (int i = 0; i < dtDataTable.Columns.Count; i++)
        {
          if (!Convert.IsDBNull(dr[i]))
          {
            string value = dr[i].ToString();
            if (value.Contains(','))
            {
              value = String.Format("\"{0}\"", value);
              sw.Write(value);
            }
            else
            {
              sw.Write(dr[i].ToString());
            }
          }
          if (i < dtDataTable.Columns.Count - 1)
          {
            sw.Write(",");
          }
        }
        sw.Write(sw.NewLine);
      }
      sw.Close();
    }
  }
}