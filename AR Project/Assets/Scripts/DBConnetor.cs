//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using UnityEngine;
//using Newtonsoft.Json;

//using MySql.Data;
//using MySql.Data.MySqlClient;

//public class DBConnetor : MonoBehaviour
//{
//    private MySqlConnection connection;

//    // MySQL ���� ���� ����
//    private string server = "bola.iptime.org";       // ���� �ּ�
//    private string database = "metabus"; // �����ͺ��̽� �̸�
//    private string user = "allione";     // ����� �̸�
//    private string password = "allione1234"; // ��й�ȣ
//    private string port = "33306";              // ��Ʈ��ȣ (�⺻ 3306)

//    // �����ͺ��̽� ���� ����
//    public void OpenConnection()
//    {
//        if (connection == null || connection.State == System.Data.ConnectionState.Closed)
//        {
//            try
//            {
//                string connectionString = $"Server={server};Database={database};User ID={user};Password={password};Port={port};";
//                connection = new MySqlConnection(connectionString);

//                connection.Open();
//                Debug.Log("MySQL connection opened.");
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError("Error opening MySQL connection: " + ex.ToString());
//            }
//        }
//    }

//    // �����ͺ��̽� ���� �ݱ�
//    public void CloseConnection()
//    {
//        if (connection != null && connection.State == System.Data.ConnectionState.Open)
//        {
//            try
//            {
//                connection.Close();
//                Debug.Log("MySQL connection closed.");
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError("Error closing MySQL connection: " + ex.Message);
//            }
//        }
//    }

//    // SELECT ���� (������ �б�)
//    public void ExecuteSelectQuery(string query)
//    {
//        if (connection != null && connection.State == System.Data.ConnectionState.Open)
//        {
//            MySqlCommand cmd = new MySqlCommand(query, connection);
//            Debug.Log($"���� : {query}");

//            try
//            {
//                MySqlDataReader dataReader = cmd.ExecuteReader();

//                // ����� ������ ����Ʈ ���� (�� ���� Dictionary ���·� ����)
//                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

//                // ��� ������ �� ���� �ݺ� ó��
//                while (dataReader.Read())
//                {
//                    Dictionary<string, object> row = new Dictionary<string, object>();

//                    // ���� ���� �� ���� �ݺ� ó��
//                    for (int i = 0; i < dataReader.FieldCount; i++)
//                    {
//                        string columnName = dataReader.GetName(i);
//                        object columnValue = dataReader.IsDBNull(i) ? null : dataReader.GetValue(i);

//                        // Dictionary�� �� �̸��� �� �߰�
//                        row.Add(columnName, columnValue);
//                    }

//                    // �� ���� ����Ʈ�� �߰�
//                    rows.Add(row);
//                }

//                // MySqlDataReader �ݱ�
//                dataReader.Close();

//                // ����Ʈ�� JSON ���ڿ��� ��ȯ
//                string jsonResult = JsonConvert.SerializeObject(rows, Formatting.Indented);

//                // JSON ��� ��� (�α׿� ���)
//                Debug.Log("JSON Result: " + jsonResult);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError("Error executing SELECT query: " + ex.Message);
//            }
//        }
//        else
//        {
//            Debug.LogError("Connection is not open.");
//        }
//    }

//    // INSERT/UPDATE/DELETE ���� (������ ����)
//    public void ExecuteNonQuery(string query)
//    {
//        if (connection != null && connection.State == System.Data.ConnectionState.Open)
//        {
//            MySqlCommand cmd = new MySqlCommand(query, connection);

//            try
//            {
//                int rowsAffected = cmd.ExecuteNonQuery();
//                Debug.Log("Query executed. Rows affected: " + rowsAffected);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError("Error executing query: " + ex.Message);
//            }
//        }
//        else
//        {
//            Debug.LogError("Connection is not open.");
//        }
//    }

//    // ���� ���� ȣ��
//    private void Start()
//    {
//        // �����ͺ��̽� ����
//        OpenConnection();

//        // ���÷� SELECT ���� ���� (�ʿ信 �°� ���� ����)
//        string selectQuery = "SELECT * FROM device";
//        ExecuteSelectQuery(selectQuery);

//        // ���÷� INSERT ���� ���� (�ʿ信 �°� ���� ����)
//        //string insertQuery = "INSERT INTO your_table_name (column1, column2) VALUES ('value1', 'value2')";
//        //ExecuteNonQuery(insertQuery);

//        // �����ͺ��̽� ���� �ݱ�
//        CloseConnection();
//    }
//}
