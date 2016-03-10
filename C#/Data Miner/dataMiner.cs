		private void getBoundsList()
		{
			listView1.Clear();
			ArrayList arrRows = new ArrayList();

			string path = browseFile("Text documents (*.txt)|*.txt", "txt");

			if (path == "")
				return;

			TextReader tr = new StreamReader(path);
			string row = tr.ReadLine();
			int nRowCount = 0;

			while (tr.Peek() >= 0) 
			{
				row = tr.ReadLine();
				
				arrRows.Add(row);
			}

			nRowCount = fillListView(ref listView1, ref arrRows, 3, '\t');

			tr.Close();

			oStatus.Panels[0].Text = nRowCount + " rows added";

		}

		private void btnQryWigle_Click(object sender, System.EventArgs e)
		{
			long downloaded = 0;
			string dlBuffer = "";
			string filename = tbWigle.Text + "Wigle!" + tbState.Text;
			
			if (tbMinLong.Text.Length > 0 || tbMaxLong.Text.Length > 0)
				filename += " (" + tbMinLong.Text + " " + tbMaxLong.Text + ")";
			
			filename += ".csv";
			
			lblInfo.Text = "Downloading " + filename;

			if ((tbState.Text.Length +
				tbMinLong.Text.Length + 
				tbMaxLong.Text.Length + 
				tbMinLat.Text.Length + 
				tbMaxLat.Text.Length) == 0)
			{
				ArrayList arrItems = new ArrayList();

				foreach (object item in checkedListBox1.CheckedItems)
				{
					tbState.Text = item.ToString();

					filename = tbWigle.Text + "Wigle!" + tbState.Text + ".csv";

					lblInfo.Text = "Downloading " + filename;

					dlBuffer = qryWigleDb(tbMinLat.Text, tbMaxLat.Text, tbMinLong.Text, tbMaxLong.Text, tbState.Text, tbSSID.Text, filename);
					
					downloaded = dlBuffer.Length;
					oStatus.Panels[0].Text = "Downloaded " + downloaded + " bytes [" + filename + "]";

					if (checkBox1.Checked)
					{
						// Add finished ones to the list
						arrItems.Clear();
 						arrItems.Add("");
 						arrItems.Add("");
 						arrItems.Add(tbState.Text);
						arrItems.Add("ok");
						arrItems.Add(Convert.ToString(downloaded));
						arrItems.Add(filename);

						addItemsToListView(ref listView2, arrItems);
					}
				}

				tbState.Text = "";
			}
			else
			{
				lblInfo.Text = "Downloading " + filename;

				dlBuffer = qryWigleDb(tbMinLat.Text, tbMaxLat.Text, tbMinLong.Text, tbMaxLong.Text, tbState.Text, tbSSID.Text, filename);

				downloaded = dlBuffer.Length;
			}

			oStatus.Panels[0].Text = "Downloaded " + downloaded + " bytes [" + filename + "]";
			lblInfo.Text = "";
		}

		private void btnGrabWigle_Click(object sender, System.EventArgs e)
		{
			string filename;
			string dlBuffer = "";
			string[] arrTemp;
			long downloaded = 0, dlBytes = 0;
			float lonDiff = 0, lonPiece = 0, _lon1 = 0, _lon2 = 0;
			string lat1, lat2, lon1, lon2;
			int splitCount = 0;
			int partIndex = 0;
			ArrayList arrItems = new ArrayList();

			string stateFileName = "";
			string allStatesFileName = @tbWigle.Text + "AllStates_" + DateTime.Now.ToShortDateString() + ".wigle";

			StreamWriter srState = null;
			StreamWriter srAllStates = 
				File.Exists(@allStatesFileName) ? 
				File.AppendText(allStatesFileName) : 
				File.CreateText(allStatesFileName);

			foreach (ListViewItem item in listView1.CheckedItems)
			{
				stateFileName = tbWigle.Text + item.SubItems[3].Text + ".state";

				splitCount = Convert.ToInt32(tbSplit.Text);

				lat1 = item.SubItems[7].Text;
				lat2 = item.SubItems[5].Text;
				lon1 = item.SubItems[8].Text;
				lon2 = item.SubItems[6].Text;

				arrTemp = item.SubItems[2].Text.Split(',');

				if (lon1 != "" && lon2 != "")
				{
					// maxlon - minlon
					lonDiff = Convert.ToSingle(lon2) - Convert.ToSingle(lon1);

					lonPiece = lonDiff / splitCount;

					_lon1 = Convert.ToSingle(lon1);
					_lon2 = _lon1 + lonPiece;
				}
				else
				{
					_lon1 = Convert.ToSingle(lon1);
					_lon2 = Convert.ToSingle(lon2);
				}

				while (splitCount > 0)
				{
					partIndex++;

					filename = tbWigle.Text + "Wigle!" + item.SubItems[0].Text + "-" + arrTemp[0] + "_" + arrTemp[1].Trim() + " (" + _lon1.ToString() + " " + _lon2.ToString() + ")." + partIndex ;
					
					oStatus.Panels[0].Text = "Grabbing " + filename;
					lblInfo.Text = "Downloading " + filename;

					tbMinLat.Text = lat1;
					tbMaxLat.Text = lat2;
					tbMinLong.Text = _lon1.ToString();
					tbMaxLong.Text = _lon2.ToString();

					do
					{
						Application.DoEvents();
						dlBuffer = qryWigleDb(lat1, lat2, _lon1.ToString(), _lon2.ToString(), item.SubItems[3].Text, tbSSID.Text, filename);						
						
						srAllStates.Write(dlBuffer);

						if (File.Exists(@stateFileName) == false)
						{
							if (srState != null)
								srState.Close();

							srState = File.CreateText(@stateFileName);
						}

						if (srState != null)
							srState.Write(dlBuffer);

						downloaded = dlBuffer.Length;
						Application.DoEvents();
					} 
					while(downloaded == 0);

					arrItems.Clear();
					arrItems.Add(item.SubItems[0].Text);
					arrItems.Add(arrTemp[1].Trim());
					arrItems.Add(arrTemp[0]);
					arrItems.Add("ok");
					arrItems.Add(Convert.ToString(downloaded));
					arrItems.Add(filename);

					addItemsToListView(ref listView2, arrItems);

					item.Checked = false;

					_lon1 += lonPiece;
					_lon2 = _lon1 + lonPiece;  // crashes if lon2 undefined here

					splitCount--;

					dlBytes += downloaded;
				}

				oStatus.Panels[0].Text = "Downloaded " + dlBytes + " as total";				
				lblInfo.Text = "";
			}

			srState.Close();
			srAllStates.Close();
		}

		public int fillListView(ref ListView lv, ref ArrayList arrRows, int stateColIndex, char delim)
		{
			setColumns();

			ArrayList arrSelStates = new ArrayList();

			ListViewItem lvItem;
			string[] arrColumns;
			int nRowCount = 0;

			if (stateColIndex > -1)
			{
				foreach (object item in checkedListBox1.CheckedItems)
				{
					arrSelStates.Add(item.ToString());
				}

				if (arrSelStates.Count == 0)
				{
					MessageBox.Show("You have to select at least 1 state from the filter tab before continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return 0;
				}
			}

			foreach (string strRow in arrRows)
			{
				arrColumns = strRow.Split(delim);

				if (stateColIndex > -1)
				{
					// filter selected states only
					if (arrSelStates.Contains(arrColumns[stateColIndex]) == false)
						continue;
				}

				lvItem = new ListViewItem(arrColumns);

				lv.Items.Add(lvItem);

				nRowCount++;
			}

			lv.EnsureVisible(1);

			return nRowCount;
		}

		private long grabData(string url, string filename, ref string strResult)
		{
			if (this.oHttp == null)
				this.oHttp = new ConHttp();

			ConHttp loHttp = this.oHttp;

			// added lately
			loHttp.AddPostKey("credential_0", "dat1");
			loHttp.AddPostKey("credential_1", "dat2");
			loHttp.AddPostKey("noexpire", "1");
			loHttp.AddPostKey("destination", url);//"/gpsopen/gps/GPSDB/confirmquery");
 
			loHttp.HandleCookies = true;

			url = wigleURL + "/gps/gps/GPSDB/login/";

			// Cookie
			Cookie _cookie = new Cookie();

			_cookie.Name = "auth";
			_cookie.Value = "xxx%3A6748578374228038800%3AW4DTIQPYMJnXGI1qMODA%2BQ";
			_cookie.Domain = ".wigle.net";
			_cookie.Path = "/";
			_cookie.Expires = Convert.ToDateTime("28-Sep-2015");

			loHttp.Cookies.Add(_cookie);

			loHttp.OnReceiveData += new ConHttp.OnReceiveDataHandler(this.loHttp_OnReceiveData);

			long lnSize = loHttp.GetUrlEvents(url, (16 * 1024), filename, ref strResult);
		
			if (loHttp.Error) 
			{
			}

			loHttp.OnReceiveData -= new ConHttp.OnReceiveDataHandler(this.loHttp_OnReceiveData);

			return lnSize;
		}

		private void loHttp_OnReceiveData(object sender, ConHttp.OnReceiveDataEventArgs e) 
		{
			if (e.Done)
			{
			}
			else if (e.CurrentByteCount >  200000000) 
			{
				MessageBox.Show("Cancelling... download is too large.");
				e.Cancel = true;
			}
			else
			{
				this.oStatus.Panels[0].Text = "Downloaded " + e.CurrentByteCount.ToString() + " bytes..";// + e.TotalBytes.ToString() + " read";
				
				Application.DoEvents();
			}
		}

		private bool detectAirport(string data)
		{
			ArrayList arrKeywords = new ArrayList();

			string[] arr =
				{
					"Airport", "airport", "AIRPORT", "Air port", "air port", "AIR PORT", "AirPort",
					"Airlines", "airlines", "AIRLINES", "Air lines", "air lines", "AIR LINES", "AirLines",
					"Airline", "airline", "AIRLINE", "Air line", "air line", "AIR LINE", "AirLine",
					"Airways", "AirWays", "Air ways", "AIRWAYS", "airways",
					"DAL CLUB", "DAL Club", "Dal Club", "dal club",
					"AA CLUB", "AA Club", "aa club",
					"UAL CLUB", "UAL Club", "Car Rental"
				};

			arrKeywords.AddRange(arr);

			foreach (string item in arrKeywords)
			{
				if (data == item || data.IndexOf(item) > -1)
					return true;
			}

			return false;
		}

		private void parseProviderDataToTableFromStream(string data, ref ListView lv, bool fromFile)
		{
			//type;country;state;city;name;address;zipcode;phonenumber;note;areacode

			ArrayList arrRows = new ArrayList();
			TextReader tr;
			
			if (fromFile)
				tr = new StreamReader(@data);
			else
				tr = new StringReader(data);

			string row = tr.ReadLine();

			while (tr.Peek() >= 0) 
			{
				row = tr.ReadLine();

				if (isAddressAirport(row, ';') == true)
				{
					if (cbAirports.Checked == false)
						continue;
				}
				else
				{
					if (cbNormAddy.Checked == false)
						continue;
				}
				
				arrRows.Add(row);
			}

			int nRowCount = fillListView(ref lv, ref arrRows, 2, ';');

			tr.Close();

			oStatus.Panels[0].Text = nRowCount + " rows added";
		}

		private void parseWigleDataToTableFromStream(string data, ref ListView lv, bool fromFile)
		{
			ArrayList arrRows = new ArrayList();
			TextReader tr;
			
			if (fromFile)
				tr = new StreamReader(@data);
			else
				tr = new StringReader(data);

			string row = tr.ReadLine();

			while (tr.Peek() >= 0) 
			{
				row = tr.ReadLine();
				
				arrRows.Add(row);
			}

			fillListView(ref lv, ref arrRows, -1, '~');

			tr.Close();
		}

		// loads line based raw address file to query gps coordinates and generate src file (read method 1)
		private void parseTMobileDataToTable(string filename)
		{
			ArrayList arrRows = new ArrayList();
			TextReader tr = new StreamReader(@filename);

			string row;
			StringBuilder sb = new StringBuilder();

			while (tr.Peek() >= 0) 
			{
				// type
				sb.Append(tr.ReadLine() + "~");

				// city - name
				row = tr.ReadLine();

				string[] arrTemp = row.Split('%');
				
				sb.Append(arrTemp[0].Trim() + "~"); // city
				sb.Append(arrTemp[1].Trim() + "~"); // name

				// addy
				sb.Append(tr.ReadLine() + "~");

				// city, state zip
				row = tr.ReadLine();
				arrTemp = row.Split(',');
				arrTemp = arrTemp[1].Trim().Split(' ');

				sb.Append(arrTemp[0].Trim() + "~"); // state
				sb.Append(arrTemp[1].Trim()); // zip

				row = tr.ReadLine(); // blank row

				arrRows.Add(sb.ToString());
				sb.Remove(0, sb.Length);
			}

			fillListView(ref listView3, ref arrRows, -1, '~');

			// close the stream
			tr.Close();
		}

		private ArrayList parseGeocoderResult(string rawData)
		{
			string[] arrTemp, arrAddress;
			ArrayList arrResult = new ArrayList();

			arrTemp = rawData.Split('$');

			for (int c=1; c<arrTemp.Length-1; c++)
			{
				arrAddress = arrTemp[c].Trim().Split('@');

				arrResult.AddRange(arrAddress);					
			}

			return arrResult;
		}

		private ArrayList parseTerraResult(string rawData)
		{
			string[] arrTemp, arrAddress;
			ArrayList arrResult = new ArrayList();

			int index = rawData.IndexOf("Lon=");

			if (index == -1)
			{
				arrResult.Add("not found");
				arrResult.Add("0");
				arrResult.Add("0");

				return arrResult;
			}

			rawData = rawData.Remove(0, index);

			arrResult.Add("Provided by Terraserver");
			arrTemp = rawData.Split('&');

			arrAddress = arrTemp[1].Split('=');
			arrResult.Add(arrAddress[1]);

			arrAddress = arrTemp[0].Split('=');
			arrResult.Add(arrAddress[1]);

			return arrResult;
		}

		private string queryGeocodes(string address)
		{
			if (this.oHttp == null)
				this.oHttp = new ConHttp();

			ConHttp loHttp = this.oHttp;
			loHttp.AddPostKey("address", address);
			
			string strResult = "";

			grabData("http://localhost/geocoder.us/demo.cgi", "", ref strResult);

			return strResult;
		}

		private string queryTerraServer(string address, string city, string state, string zipcode)
		{
			string strResult = "", request = 
				"http://terraserver-usa.com/advfind.aspx?" +
				"__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=dDwtOTc3NjUxOTM0OztsPFBnU3JoOk5hdkNvdmVyYWdlO0FGU2VhcmNoOz4%2B&" +
				"PlaceId=&LastPlace=&SearchPlace=&QueryId=&SearchState=&PlaceTypeId=&SearchCountry=" +
				"&AFStreet=" + address +
				"&AFCity=" + city +
				"&AFState=" + state +
				"&AFCountry=USA" +
				"&AFZipCode=" + zipcode +
				"&AFSearch.x=15&AFSearch.y=19";

			grabData(request, "", ref strResult);
			//grabData("http://terraserver-usa.com/advfind.aspx", "", ref strResult);

			return strResult;
		}

		private string Login()
		{
			if (this.oHttp == null)
				this.oHttp = new ConHttp();

			ConHttp loHttp = this.oHttp;
			loHttp.AddPostKey("credential_0", "mydev");
			loHttp.AddPostKey("credential_1", "mypass");
			loHttp.AddPostKey("noexpire", "1");
			loHttp.AddPostKey("destination", "/gps/gps/GPSDB/query/");

			loHttp.HandleCookies = true;
			string strResult = "";

			//string request = wigleURL + "/gpsopen/gps/GPSDB/login/?credential_0=mydev&credential_1=mypass&noexpire=1&destination=/gps/gps/";

			//grabData(request, "", ref strResult);
			//grabData("http://wigle.net/gps/gps/GPSDB/login/", "", ref strResult);
			grabData(wigleURL + "/gps/gps/GPSDB/login/", "", ref strResult);

			textBox1.Text = strResult;

			return strResult;
		}

		private string validateState(string state)
		{
			string[] states_all =
			{
				"Alabama", "AL", "Alaska", "AK", "American Samoa", "AS", "Arizona", "AZ", "Arkansas", "AR", "California", "CA",
				"Colorado", "CO", "Connecticut", "CT", "Delaware", "DE", "District of Columbia", "DC", "Federated States of Micronesia", "FM",
				"Florida", "FL", "Georgia", "GA", "Guam", "GU", "Hawaii", "HI", "Idaho", "ID", "Illinois", "IL", "Indiana", "IN",
				"Iowa", "IA", "Kansas", "KS", "Kentucky", "KY", "Louisiana", "LA", "Maine", "ME", "Marshall Islands", "MH", "Maryland", "MD",
				"Massachusetts", "MA", "Michigan", "MI", "Minnesota", "MN", "Mississippi", "MS", "Missouri", "MO", "Montana", "MT",
				"Nebraska", "NE", "Nevada", "NV", "New Hampshire", "NH", "New Jersey", "NJ", "New Mexico", "NM", "New York", "NY",
				"North Carolina", "NC", "North Dakota", "ND", "Northern Mariana Islands", "MP", "Ohio", "OH", "Oklahoma", "OK", "Oregon", "OR",
				"Palau", "PW", "Pennsylvania", "PA", "Puerto Rico", "PR", "Rhode Island", "RI", "South Carolina", "SC", "South Dakota", "SD",
				"Tennessee", "TN", "Texas", "TX", "Utah", "UT", "Vermont", "VT", "Virgin Islands", "VI", "Virginia", "VA", "Washington", "WA",
				"West Virginia", "WV", "Wisconsin", "WI", "Wyoming", "WY"
			};

			if (state.Length == 2)
				return state;

			for (int c=0; c<states_all.Length; c++)
			{
				if (states_all[c].ToLower() == state.ToLower())
					return states_all[c+1];
			}

			return state;
		}

		private string validateAddress(string address)
		{
			int cutIndex;
			string result = "", _data = "", addyBack = " ";
			//ArrayList arrAddy = new ArrayList();
			ArrayList arrKeywords = new ArrayList();

			string[] cutEndWords =
				{
					"STREET", "ST", "ST.", "AVENUE", "AVE", "AVE.", "DR.", "DRIVE", "BLVD", "BLVD.", "RD.", "RD", "ROAD",
					"PKWY", "PARKWAY", "BOULEVARD", "HWY", "DRÝVE",
					"SPACE", "SUITE", "STE", "BLDG", "BLDG.", "UNIT",

					/*, "EAST", "WEST", "NORTH", "SOUTH", "N", "W", "S", "E", "NW", "NE", "SW", "SE"*/
				};


			for (int c=0; c<cutEndWords.Length; c++)
			{
				addyBack = address.ToUpper() + " ";

				cutIndex = addyBack.IndexOf(" " + cutEndWords[c] + " ");

				if (cutIndex > -1)
				{
					int nStart = cutIndex + cutEndWords[c].Length + 2;

					if (nStart < address.Length)
						address = address.Remove(nStart , address.Length - nStart);
				}
			}


//			// truncate from "SPACE "
//			cutIndex = address.IndexOf("Space ");
//
//			if (cutIndex > -1)
//				address = address.Remove(cutIndex, address.Length - cutIndex);
//
//			// truncate from "SUITE "
//			cutIndex = address.IndexOf("Suite ");
//
//			if (cutIndex > -1)
//				address = address.Remove(cutIndex, address.Length - cutIndex);
//
//			// truncate from "STE "
//			cutIndex = address.IndexOf("Ste ");
//
//			if (cutIndex > -1)
//				address = address.Remove(cutIndex, address.Length - cutIndex);
//
//			// truncate from "BLDG"
//			cutIndex = address.IndexOf("BLDG");
//
//			if (cutIndex > -1)
//				address = address.Remove(cutIndex, address.Length - cutIndex);
//
//			// truncate from "Unit"
//			cutIndex = address.IndexOf("Unit");
//
//			if (cutIndex > -1)
//				address = address.Remove(cutIndex, address.Length - cutIndex);

			// truncate from #
			cutIndex = address.IndexOf("#");

			if (cutIndex > -1)
				address = address.Remove(cutIndex, address.Length - cutIndex);

			// truncate from (
			cutIndex = address.IndexOf("(");

			if (cutIndex > -1)
				address = address.Remove(cutIndex, address.Length - cutIndex);



			string[] arrAddy = address.Split(' ');
			string[] arrTemp = arrAddy[0].Split('-');

			// choose first of A-B of word1
			if (arrTemp.Length > 0)
				arrAddy[0] = arrTemp[0];

			// replace some other
			for (int c=0; c<arrAddy.Length; c++)
			{
				_data = arrAddy[c].Trim().ToUpper();

				if (_data == "")
					break;

				if (_data == "N.E.")
				{
					arrAddy[c] = "NE";
					continue;
				}

				if (_data == "N.W.")
				{
					arrAddy[c] = "NW";
					continue;
				}

				if (_data == "S.E.")
				{
					arrAddy[c] = "SE";
					continue;
				}

				if (_data == "S.W.")
				{
					arrAddy[c] = "SW";
					continue;
				}
			}

			foreach (string item in arrAddy)
			{
				if (item.Trim() != "")
					result += " " + item;
			}

			return result.Trim();
		}

		private string validateCity(string city)
		{
			string[] arr = city.Split('(');

			return arr[0].Trim();
		}

		private string validateZipcode(string zipcode)
		{
			for (int c=zipcode.Length; c<5; c++)
			{
				zipcode = "0" + zipcode;
			}

			return zipcode;
		}

		private string validateSSID(string provider, ref Hashtable ht, bool bBrutal)
		{
			string result = "";

			if (ht.ContainsKey(provider))
			{
				ArrayList arr = (ArrayList) ht[provider];

				foreach (string item in arr)
				{
					result += item + "~";
				}

				return result.Remove(result.Length - 1, 1);
			}

			return "*";
		}

		private ArrayList filterWigleArray(string ssid, ref ArrayList arrWigle)
		{
			ArrayList arrResults = new ArrayList();
			string[] arrSSID = ssid.Split('~');

			foreach (string[] arr in arrWigle)
			{
				foreach (string _ssid in arrSSID)
				{
					if ((arr[1].Trim().ToLower() == _ssid.Trim().ToLower()) || 
						(_ssid.Trim() == "" && arr[1].Trim().ToLower() == "<no ssid>"))
					{
						arrResults.Add(arr);
					}				
				}
			}

			return arrResults;
		}


		// compares gps data with wigle to obtain mac address(es)
		private ArrayList queryWigleArray(string ssid, float latL, float latH, float lonL, float lonH, ref ArrayList arrWigle, ref Hashtable ht, ref StreamWriter sr)
		{
			float latW, lonW;

			ArrayList arrResults = new ArrayList();
			ArrayList arrSources = new ArrayList();
			//ArrayList myArr = null;
			string[] arrSSID = ssid.Split('~');
			//string ssidMismatch;

			int resultLimit = 0, ssid_found = 0;
			
			if (tbResultLimit.Text != "")
				resultLimit = Convert.ToInt32(tbResultLimit.Text);

			foreach (string _ssid in arrSSID)
			{
				if (ht.ContainsKey(_ssid))
					arrSources.Add((ArrayList) ht[_ssid]);
			}

			arrSources.Add(arrWigle);

			foreach (ArrayList myArr in arrSources)
			{
				foreach (string[] arr in myArr/*arrWigle*/)
				{
					try
					{
						latW = Convert.ToSingle(arr[10]);
						lonW = Convert.ToSingle(arr[11]);
						
						if ((latW <= latH && latW >= latL) && 
							(lonW <= lonH && lonW >= lonL))
						{
							ssid_found = 0;

							foreach (string _ssid in arrSSID)
							{
								if ((arr[1].Trim().ToLower() == _ssid.Trim().ToLower()) || 
									(_ssid.Trim() == "" && arr[1].Trim().ToLower() == "<no ssid>"))
								{
									ssid_found++;

									arrResults.Add(arr);

									if (resultLimit > 0 && arrResults.Count >= resultLimit) // max n results
										break;
								}
							}

//							if (ssid_found == 0)
//							{
//								ssidMismatch = arr[0] + ";" + arr[1] + ";[although GPS matched, SSID mismatch.. skipped]";
//
//								Console.WriteLine(ssidMismatch);
//								sr.WriteLine(ssidMismatch);
//							}
						}
					}
					catch (Exception exError)
					{
						Console.WriteLine(exError.Message);
					}
				}
			}

			return arrResults;
		}

		// create GPS coordinates for selected states
		private void grabGPSCoordinates()
		{
			int col_type=-1, col_name=-1, col_city=-1, col_state=-1, col_country=-1, col_address=-1, col_zipcode=-1, col_phone=-1;
			int col_description=-1, col_areacode=-1, col_provider=-1, col_price=-1, col_ssid=-1, col_pay=-1, col_latitude=-1, col_longitude=-1;
			int notFoundCount = 0, grabbedCount = 0, multipleCount = 0, currentId = 0, totalRows = 0, skippedAirport = 0, terraCount = 0;
			string curProvider = "", strRow = "";
			string /*type, city, name, address, state, zipcode,*/ qryAddress;
			string[] arrAddress, arrListview, prevRow = {};
			ArrayList arrStateAddys = new ArrayList();
			ArrayList arrResult = new ArrayList();
			ListViewItem lvItem;
			StringBuilder rsb = new StringBuilder();
			bool bSkipDup = false;

			#region indexing
			string indexFile = @tbAddress.Text + "columns.cfg";
			string _col;
			string[] _tmparr;

			if (File.Exists(indexFile) == false)
			{
				MessageBox.Show("Columns configuration file " + indexFile + " not found, process aborted..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			TextReader trIndex = new StreamReader(indexFile);

			while (trIndex.Peek() >= 0)
			{
				_col = trIndex.ReadLine();

				_tmparr = _col.Split('=');

				try
				{
					switch(_tmparr[0])
					{
						case "type":
							//						if (_tmparr[1] == "*")
							//							break;
							col_type = Convert.ToInt32(_tmparr[1]);
							break;
						case "name":
							col_name= Convert.ToInt32(_tmparr[1]);
							break;
						case "city":
							col_city= Convert.ToInt32(_tmparr[1]);
							break;
						case "state":
							col_state= Convert.ToInt32(_tmparr[1]);
							break;
						case "country":
							col_country= Convert.ToInt32(_tmparr[1]);
							break;
						case "address":
							col_address= Convert.ToInt32(_tmparr[1]);
							break;
						case "zipcode":
							col_zipcode= Convert.ToInt32(_tmparr[1]);
							break;
						case "phone":
							col_phone= Convert.ToInt32(_tmparr[1]);
							break;
						case "description":
							col_description= Convert.ToInt32(_tmparr[1]);
							break;
						case "areacode":
							col_areacode= Convert.ToInt32(_tmparr[1]);
							break;
						case "provider":
							col_provider= Convert.ToInt32(_tmparr[1]);
							break;
						case "price":
							col_price= Convert.ToInt32(_tmparr[1]);
							break;
						case "ssid=":
							col_ssid= Convert.ToInt32(_tmparr[1]);
							break;
						case "pay":
							col_pay= Convert.ToInt32(_tmparr[1]);
							break;
						case "latitude":
							col_latitude= Convert.ToInt32(_tmparr[1]);
							break;
						case "longitude":
							col_longitude= Convert.ToInt32(_tmparr[1]);
							break;
					}
				}
				catch
				{
				}
			}
	
			#endregion


			if (providerValidation() == false)
				return;


			// providers
			foreach (object _provider in checkedListBox3.CheckedItems)
			{
				curProvider = _provider.ToString();
				grabbedCount = notFoundCount = terraCount = 0;
				arrStateAddys.Clear();

				string srcFile = tbAddress.Text + curProvider + "_src.csv";

				if (File.Exists(srcFile) == false)
				{
					MessageBox.Show("File " + srcFile + " not found, skipping", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					break;
				}


				TextReader trAddress = new StreamReader(@srcFile); // tmobile_src.csv

				#region validate, filter & collect datas by STATE
				while (trAddress.Peek() >= 0) 
				{
					arrAddress = trAddress.ReadLine().Split(Convert.ToChar(tbDelimiter.Text));

					try
					{
						arrAddress[col_state] = validateState(arrAddress[col_state].Trim());
					}
					catch
					{
						continue;
					}

					// filter state
					if (checkedListBox1.CheckedItems.Contains(arrAddress[col_state].ToUpper()) && arrAddress[col_country] == "USA")
					{
						if (isAddressAirport(arrAddress) == true)
						{
							if (cbAirports.Checked == false)
							{
								skippedAirport++;
								continue;
							}
						}
						else
						{
							if (cbNormAddy.Checked == false)
								continue;
						}

						arrAddress[col_city] = validateCity(arrAddress[col_city]);
						arrAddress[col_address] = validateAddress(arrAddress[col_address]);
						arrAddress[col_zipcode] = validateZipcode(arrAddress[col_zipcode]);

						if (arrStateAddys.Contains(arrAddress) == false)
							arrStateAddys.Add(arrAddress);
						else
							Console.WriteLine(arrAddress[col_address] + " skipped as duplicate..");
					}
				}

				trAddress.Close();
				#endregion

               
				totalRows = arrStateAddys.Count;
				currentId = 0;

				StreamWriter sr = File.CreateText(@tbAddress.Text + curProvider + "_gps.csv"); // tmobile_gps.csv

				foreach (string[] arr in arrStateAddys)
				{
					if (bAbortProcess)
					{
						bAbortProcess = false;
						break;
					}

					currentId++;

					if (prevRow.Length > 0)
					{
						// dup check
						if (prevRow[col_state]+prevRow[col_city]+prevRow[col_address]+prevRow[col_zipcode] == arr[col_state]+arr[col_city]+arr[col_address]+arr[col_zipcode])
							bSkipDup = true;
					}

                    prevRow = arr;	

					// type0;country1;state2;city3;name4;address5;zipcode6;phonenumber7;note8;areacode9;provider10;pricing11;ssid12;pay13;addy14;lat15;lon16
					
					
					#region prepare
					rsb.Length = 0;

					// DO NOT CHANGE THE ORDER!!
					if (col_type != -1) rsb.Append(arr[col_type] + "~"); else rsb.Append("~");
					if (col_country != -1) rsb.Append(arr[col_country] + "~"); else rsb.Append("~");
					if (col_state != -1) rsb.Append(arr[col_state] + "~"); else rsb.Append("~");
					if (col_city != -1) rsb.Append(arr[col_city] + "~"); else rsb.Append("~");
					if (col_name != -1) rsb.Append(arr[col_name] + "~"); else rsb.Append("~");
					if (col_address != -1) rsb.Append(arr[col_address] + "~"); else rsb.Append("~");
					if (col_zipcode != -1) rsb.Append(arr[col_zipcode] + "~"); else rsb.Append("~");
					if (col_phone != -1) rsb.Append(arr[col_phone] + "~"); else rsb.Append("~");
					if (col_description != -1) rsb.Append(arr[col_description] + "~"); else rsb.Append("~");
					if (col_areacode != -1) rsb.Append(arr[col_areacode] + "~"); else rsb.Append("~");
					
					
					if (tbForceProvider.Text != "") // provider
						rsb.Append(tbForceProvider.Text + "~");
					else
					{
						if (cbProviderEach.Checked == true)
							rsb.Append(curProvider + "~");
						else
							if (col_provider != -1) rsb.Append(arr[col_provider] + "~"); else rsb.Append("~");
					}
					
					if (tbForcePrice.Text != "") // price
						rsb.Append(tbForcePrice.Text + "~");
					else
						if (col_price != -1) rsb.Append(arr[col_price] + "~"); else rsb.Append("~");

					if (col_ssid != -1) rsb.Append(arr[col_ssid] + "~"); else rsb.Append("~"); // ssid

					if (tbForcePay.Text != "") // pay
						rsb.Append(tbForcePay.Text + "~");
					else
						if (col_pay != -1) rsb.Append(arr[col_pay] + "~"); else rsb.Append("~");
					

					strRow = rsb.ToString();

					qryAddress = arr[col_address] + ", " + arr[col_city] + " " + arr[col_state];// + " " + arr[col_zipcode];

					lblInfo.Text = "Querying [" + currentId + "/" + totalRows + "]  " + qryAddress;
					#endregion

					#region qry
					if (bSkipDup == false)
					{
						if (cbUseExistingGPS.Checked == true && arr[col_latitude] != "" && arr[col_longitude] != "")
						{
							arrResult.Clear();

							arrResult.Add("Used existing GPS");
							arrResult.Add(arr[col_latitude]);
							arrResult.Add(arr[col_longitude]);
						}
						else
						{
							if (cbTerra.Checked == true)
								arrResult = parseTerraResult(queryTerraServer(arr[col_address], arr[col_city], arr[col_state], arr[col_zipcode]));
							else
							{
								arrResult = parseGeocoderResult(queryGeocodes(qryAddress));

								if (arrResult.Count == 0)
								{
									arrResult = parseTerraResult(queryTerraServer(arr[col_address], arr[col_city], arr[col_state], arr[col_zipcode]));
									arrResult[0] += " - Not Found!";
									terraCount++;
								}

								if (arrResult.Count > 3)
								{
									if (rbUseTerra.Checked == true)
									{
										arrResult = parseTerraResult(queryTerraServer(arr[col_address], arr[col_city], arr[col_state], arr[col_zipcode]));
									}
									else
									{
	//									arrResult[0] += " - Multiple!";
										multipleCount++;

	//									terraCount++;
									}
								}
							}
						}
					}
					else
						bSkipDup = false;
					#endregion

					#region append to file
					string strMultiple = "";

					if (arrResult.Count > 0)
					{
						for (int c=0; c<arrResult.Count;)
						{
							if (arrResult.Count > 3)
							{
								strMultiple = strRow + "[" + arrResult[c+0] + " - Multiple!]~" + arrResult[c+1] + "~" + arrResult[c+2];
								oStatus.Panels[0].Text = "Multiple GPS found: " + arr[col_address] + "[" + arr[col_state] + "]";

								sr.WriteLine(strMultiple);
							}
							else
							{
								strRow += "[" + arrResult[c]  + "]~" + arrResult[c+1] + "~" + arrResult[c+2];
								oStatus.Panels[0].Text = "Grabbed: " + arrResult[c];
								grabbedCount++;

								sr.WriteLine(strRow);
							}

							c += 3;
						}

						// le idiot
						if (strMultiple != "")
							strRow = strMultiple;
					}
					else
					{
						strRow += "Address Not found !!~0~0";
						notFoundCount++;

						oStatus.Panels[0].Text = "Address Not found: " + arr[col_address] + "[" + arr[col_state] + "]";

						sr.WriteLine(strRow);
					}
					#endregion

					
					Application.DoEvents();

					if (cbShowResults.Checked == true)
					{
						// update listview
						arrListview = strRow.Split('~');

						lvItem = new ListViewItem(arrListview);
						listView3.Items.Add(lvItem);

						lvItem.EnsureVisible();

						//listView3.Update();
						//this.Update();
					}
				}

				sr.Close();

				oStatus.Panels[0].Text = curProvider + " - " + grabbedCount + " grabbed, " + terraCount + " used from terraserver, " +
					skippedAirport + " airport skipped, " /*+ multipleCount + " multiple gps found, "*/ + notFoundCount + " not found";

//				MessageBox.Show(oStatus.Panels[0].Text, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

//				if (cbAutoMatch.Checked == true)
//				{
//					tabControl1.TabPages[2].Select();
//					matchAndMakeResults();
//				}
			}
		}

		// create locations.csv and network.csv files after matching
		private	void matchAndMakeResults()
		{
			#region variables and initializations
			ArrayList arrNetRows = new ArrayList();
			ArrayList arrWigle = new ArrayList();
			ArrayList arrSmallSSID = new ArrayList();
			ArrayList arrWigleTemp = new ArrayList();
			ArrayList arrRadius = new ArrayList();
			ArrayList arrResults = null, arrSSID = null, arrNetwork = null;
			ArrayList arrProcessed = new ArrayList();
			ArrayList arrMatched = new ArrayList();
			ArrayList arrNotFound = new ArrayList();
			ArrayList arrMultiple = new ArrayList();
			ArrayList arrDuplicate = new ArrayList();

			
			//ArrayConverter ac = new ArrayConverter();

			Hashtable htWigle_SSID = new Hashtable();
			Hashtable htMatched_locs = new Hashtable();
			Hashtable htMatched_nets = new Hashtable();
			Hashtable htMultiple_locs = new Hashtable();
			Hashtable htMultiple_nets = new Hashtable();
			Hashtable htMatched_address = new Hashtable();
			//TextReader strReader = new StringReader();

			ListViewItem lvItem;

			float[] arrRadVals = {0.001f, 0.002f, 0.005f, 0.010f, 0.020f, 0.050f, 0.100f, 0.200f};

			arrRadius.AddRange(arrRadVals);

			string row, usedRadius = "", strRad = "", curState = "", curProvider = "", curGPS = "", last_ssid = "null", ssidBack = "";
			//string strLastLoc = "", last_address = "", last_provider = "", curAddress = "", rawData = "", isFree = "";
			float lat, lon, latRL, lonRL, latRH, lonRH/*, lastRadius = 0*/;

			int nProcessedAddresses = 0, nFoundNetwork = 0, nFoundLocations = 0, nFoundDuplicate = 0, nNotFoundLocations = 0;
			int loc_id = 1, net_id = 0/*, multiple_loc_id = 0*/, curLoc_id = 0;

			int col_type=0, col_name=4, col_city=3, col_state=2, /*col_country=1, */col_address=5, col_zipcode=6, col_phone=7;
			int col_description=8, col_areacode=9, col_provider=10, col_price=11, col_ssid=12, col_pay=13, /*col_addy=14, */col_latitude=15, col_longitude=16;

			if (tbLocId.Text != "")
				loc_id = Convert.ToInt32(tbLocId.Text) - 1;

			if (tbNetId.Text != "")
				net_id = Convert.ToInt32(tbNetId.Text) - 1;

			StringBuilder sb = new StringBuilder();
			string[] arrAddress, _tmparrWigle, arrPrevMatched;
			bool bSkip_loc = false;


//			int latOff = Convert.ToInt32(tbLatOffset.Text);
//			int lonOff = Convert.ToInt32(tbLonOffset.Text);

			StreamWriter sr1 = null, sr1a = null, sr5 = null;
			StreamWriter sr2 = null, sr2a = null, sr3 = null, sr4 = null;

			if (providerValidation() == false)
				return;

			#region provider configuration
			Hashtable htProviders = new Hashtable();
			ArrayList arrProvider = null;
			bool bBrutal = false;

			string provFile = @tbAddress.Text + "providers.cfg";
			string _prov;
			string[] _tmparr, _arrProv, _arrSSID;

			TextReader trProv = new StreamReader(provFile);

			_prov = trProv.ReadLine();

			if (_prov.Trim().ToLower() == "noflexiblematching")
			{
				bBrutal = false;
			}
			else if (_prov.Trim().ToLower() == "flexiblematching")
			{
				bBrutal = true;
			}

			while (trProv.Peek() >= 0)
			{

				_prov = trProv.ReadLine().Trim().ToLower();

				if (_prov == "")
					continue;


				_tmparr = _prov.Split('%');

				// skip unexisting ssid ones
				if (_tmparr.Length == 1)
					continue;

				_arrProv = _tmparr[0].Split('~');
				_arrSSID = _tmparr[1].Split('~');


				foreach (string provItem in _arrProv)
				{
					if (htProviders.ContainsKey(provItem))
						arrProvider = (ArrayList) htProviders[provItem];
					else
						arrProvider = new ArrayList();

				
					foreach (string ssidItem in _arrSSID)
					{
						arrProvider.Add(ssidItem);						
					}

					if (htProviders.ContainsKey(provItem) == false)
						htProviders.Add(provItem, arrProvider);
				}

			}

			#endregion

			setColumns();
			#endregion

			// providers
			foreach (object _provider in checkedListBox3.CheckedItems)
			{
				#region creating files, init arrays
				curProvider = _provider.ToString();

				nFoundDuplicate = 0;
				nFoundLocations = 0;
				nFoundNetwork = 0;
				nNotFoundLocations = 0;
				nProcessedAddresses = 0;


				// use single out files for whole states
				// todo check file existing to append
				string destFilePrefix = tbLocations.Text + "/" + curProvider + "_";

				if (File.Exists(@destFilePrefix + "locations.csv") ||
					File.Exists(@destFilePrefix + "network.csv"))
				{
					DialogResult dr = MessageBox.Show(curProvider + "'s locations.csv or network.csv files already exists, [YES] - Overwrite existing files, [NO] - Append existing files", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

					switch(dr)
					{
						case DialogResult.Yes:
							sr1 = File.CreateText(@destFilePrefix + "locations.csv");
							sr2 = File.CreateText(@destFilePrefix + "network.csv");
							sr3 = File.CreateText(@destFilePrefix + "locs_notfound.csv");
							sr4 = File.CreateText(@destFilePrefix + "ssid_mismatches.csv");
							sr5 = File.CreateText(@destFilePrefix + "dup_check.csv");
							break;

						case DialogResult.No:
							sr1 = File.AppendText(@destFilePrefix + "locations.csv");
							sr2 = File.AppendText(@destFilePrefix + "network.csv");
							sr3 = File.AppendText(@destFilePrefix + "locs_notfound.csv");
							sr4 = File.AppendText(@destFilePrefix + "ssid_mismatches.csv");
							sr5 = File.AppendText(@destFilePrefix + "dup_check.csv");
							break;
					}
				}
				else
				{
					sr1 = File.CreateText(@destFilePrefix + "locations.csv");
					sr2 = File.CreateText(@destFilePrefix + "network.csv");
					sr3 = File.CreateText(@destFilePrefix + "locs_notfound.csv");
					sr4 = File.CreateText(@destFilePrefix + "ssid_mismatches.csv");
					sr5 = File.CreateText(@destFilePrefix + "dup_check.csv");
				}

				// put header columns of locations.csv
				sr1.WriteLine("location_id~type~name~city~state~country~address~zipcode~areacode~phone~description~latitude~longitude");

				// put header columns of network.csv
				sr2.WriteLine("network_id~location_id~provider~type~ssid~free?~pricing~mac address");
				#endregion


				// states
				foreach (object _state in checkedListBox1.CheckedItems)
				{
					#region files existing check, init arrays
					arrDuplicate.Clear();
					arrProcessed.Clear(); // clear dup check array for each state
					htMatched_locs.Clear();
					htMatched_nets.Clear();
					htMatched_address.Clear();

					if (bAbortProcess)
						break;

					curState = _state.ToString();

					string wigleFile = @tbWigle.Text + curState+ "_wigle.csv";
					string gpsFile = @tbAddress.Text + curProvider + "_gps.csv";

					if (File.Exists(wigleFile) == false)
					{
						MessageBox.Show("File " + wigleFile + " not found, skipping", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						continue;
					}

					if (File.Exists(gpsFile) == false)
					{
						MessageBox.Show("File " + gpsFile + " not found, skipping", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						break;
					}

					TextReader trWigle = new StreamReader(wigleFile);
					TextReader trAddress = new StreamReader(gpsFile);
					#endregion

					#region collect WIGLE STATE datas to array
					arrWigle.Clear();
					arrSmallSSID.Clear();
					htWigle_SSID.Clear();


					// wigle columns header row
					trWigle.ReadLine();

					while (trWigle.Peek() >= 0) 
					{
						_tmparrWigle = trWigle.ReadLine().Split('~');

						if (cbSkipNoSSID.Checked == true && 
							_tmparrWigle[1].ToLower().Trim() == "<no ssid>" || _tmparrWigle[1].ToLower().Trim() == "")
							continue;

						_tmparrWigle[1] = _tmparrWigle[1].ToLower();

						// filter ssid
//						if (curProvider.ToLower() == _tmparrWigle[1].ToLower()/* || _tmparrWigle[1].ToUpper() == "TMOBÝLE"*/)
//						{
//							// parse to string array and add them to data array
//							// dup check but might be slow down the process
							if (htWigle_SSID.ContainsKey(_tmparrWigle[1]))
							{
                                arrSSID = (ArrayList) htWigle_SSID[_tmparrWigle[1]];

								arrSSID.Add(_tmparrWigle);
							}
							else
							{
								arrSSID = new ArrayList();

								arrSSID.Add(_tmparrWigle);

								htWigle_SSID.Add(_tmparrWigle[1], arrSSID);
							}

//							if (_tmparrWigle.Contains(_tmparrWigle) == false)
//								_tmparrWigle.Add(_tmparrWigle);
//						}
					}

//					Hashtable httest = new Hashtable();

					foreach (string item in htWigle_SSID.Keys)
					{
						ArrayList arr = (ArrayList) htWigle_SSID[item];

						if (arr.Count < 5)
							arrSmallSSID.Add(item);
//						else
//							httest.Add(item, arr);
					}

					foreach (string item in arrSmallSSID)
					{
						ArrayList _arr = (ArrayList) htWigle_SSID[item];

						foreach (string[] _item in _arr)
						{
							arrWigle.Add(_item);						
						}

						htWigle_SSID.Remove(item);
					}



					// close the wigle stream
					trWigle.Close();
					#endregion

					#region create header rows
					if (htWigle_SSID.Count == 0 || arrWigle.Count == 0)
					{
						lblInfo.Text = "No provider data for " + curState + ", skipped..";
						continue;
					}

					// create new out files for each state separately
					if (cbSeperateStates.Checked == true)
					{
						sr1a = File.CreateText(@tbLocations.Text + curProvider + "_" + curState + "_locations.csv");
						sr2a = File.CreateText(@tbLocations.Text + curProvider + "_" + curState + "_network.csv");

						// put header columns of locations.csv
						sr1a.WriteLine("location_id~type~name~city~state~country~address~zipcode~areacode~phone~description~latitude~longitude");

						// put header columns of network.csv
						sr2a.WriteLine("network_id~location_id~provider~type~ssid~free?~pricing~mac address");
					}

//					currentId = 0;
					#endregion


					#region matching loop for each source address
					
					// type0;country1;state2;city3;name4;address5;zipcode6;phonenumber7;note8;areacode9;provider10;pricing11;ssid12;pay13;addy14;lat15;lon16

					// address db loop
					while (trAddress.Peek() >= 0)
					{
						if (bAbortProcess)
							break;

						row = trAddress.ReadLine();

						arrAddress = row.Split(Convert.ToChar(tbDelimiter.Text));

						#region preparsing, global dup check

						// force overriding provider to a single value
						if (tbForceProvider.Text.Trim() != "")
							arrAddress[col_provider] = tbForceProvider.Text.Trim();

						// filter state
//						if (checkedListBox1.CheckedItems.Contains(arrAddress[col_state].ToLower()) == false)
						if (arrAddress[col_state].ToUpper() != curState.ToUpper())
							continue;

						if (cbSkipNoSSID.Checked == true && 
							(arrAddress[col_provider].ToLower().Trim() == "independent provider" || arrAddress[col_ssid].ToLower().Trim() == ""))
							continue;

						curGPS = arrAddress[col_latitude] + ";" + arrAddress[col_longitude];

						if (htMatched_locs.ContainsKey(curGPS))
						{
							arrPrevMatched = htMatched_locs[curGPS].ToString().Split('~');

							if (arrAddress[col_provider].Trim() == arrPrevMatched[10])
								continue;
							else
							{
								bSkip_loc = true;
								curLoc_id = Convert.ToInt32(arrPrevMatched[0]);
							}
						}
						else
							bSkip_loc = false;
						#endregion


//						currentId++;

						#region validations
						lblInfo.Text = "Querying.. " + curProvider + " / " + curState + " [" +
							nProcessedAddresses + " lookup, " + nFoundLocations + " good news, " + 
							nNotFoundLocations + " bad news, " + nFoundDuplicate + " dup skipped, " +
							nFoundNetwork + " mac_id mined]";

						lat = Convert.ToSingle(arrAddress[col_latitude]);
						lon = Convert.ToSingle(arrAddress[col_longitude]);

						ssidBack = arrAddress[col_ssid].Trim().ToLower();

						// find appropriate SSID by given provider text
						arrAddress[col_ssid] = validateSSID(arrAddress[col_provider].Trim().ToLower(), ref htProviders, bBrutal);

						// * means various ssid's and will use given ssid by address
						if (arrAddress[col_ssid] == "*")
							arrAddress[col_ssid] = ssidBack;

						nProcessedAddresses++;

//						if (arrAddress[14].IndexOf("Multiple!") == -1)
//							continue;

						// if current ssid is same with previous ssid, use previously filtered source wigle ssid array (for speed up)
						if (last_ssid != arrAddress[col_ssid])
							arrWigleTemp = filterWigleArray(arrAddress[col_ssid], ref arrWigle);
						#endregion 


						#region query boundaries with increasing radius
						//foreach (float _radius in arrRadius)
						for (float _radius = 0.001f; _radius < 0.200f; _radius += 0.001f)
						{
							strRad = _radius.ToString();

							if (strRad.Length > 5)
							{
								strRad = strRad.Remove(5, strRad.Length - 5);
								_radius = Convert.ToSingle(strRad);
							}

							latRL = lat - _radius;
							latRH = lat + _radius;

							lonRL = lon - _radius;
							lonRH = lon + _radius;

							arrResults = queryWigleArray(arrAddress[col_ssid], latRL, latRH, lonRL, lonRH, ref arrWigleTemp, ref htWigle_SSID, ref sr4);

							usedRadius = _radius.ToString();

							// we found at least 1 result so this is not necessary to increase radius and retry
							// TODO: if we need at least n results we can increase radius and retry unless obtaining n results
							if (arrResults.Count > 0)
								break;
						}
						#endregion

						last_ssid = arrAddress[col_ssid];


						// we found some results so put them to associated file
						if (arrResults.Count > 0)
						{
							if (bSkip_loc == false)
							{
								#region append new row to locations.csv file
								string[] _temparr;

								if (htMatched_address.Contains(arrAddress[col_address]))
								{
									// loc_id, radius, lat;lon
									_temparr = (string[]) htMatched_address[arrAddress[col_address]];

									if (Convert.ToSingle(_temparr[1]) < Convert.ToSingle(usedRadius))
									{
										sr5.WriteLine(_temparr[0] + ";" + _temparr[2] + ";" + _temparr[1] + " < " + usedRadius + ";keep current");
										continue;	// keep existing one, skip current
									}
									else
									{
										sr5.WriteLine(_temparr[0] + ";" + _temparr[2] + ";" + _temparr[1] + " > " + usedRadius + ";replace");

										htMatched_address.Remove(arrAddress[col_address]);
										htMatched_locs.Remove(_temparr[2]);
										htMatched_nets.Remove(_temparr[0]);
										nFoundLocations--;
									}
								}


								loc_id++;

								// location_id, type, name, city, state, country, address, zipcode, areacode  phone, description, latitude, longitude
								sb.Append(loc_id + "~");
								sb.Append(arrAddress[col_type] + "~");
								sb.Append(arrAddress[col_name] + "~");	// name (starbucks)
								sb.Append(arrAddress[col_city] + "~");	// city
								sb.Append(arrAddress[col_state] + "~");	// state
								sb.Append("USA" + "~");
								sb.Append(arrAddress[col_address] + "~");	// address
								sb.Append(arrAddress[col_zipcode] + "~");	// zipcode
								sb.Append(arrAddress[col_areacode] + "~");	// areacode
								sb.Append(arrAddress[col_phone] + "~");	// phone
								sb.Append(arrAddress[col_description] + "~");	// description
								sb.Append(arrAddress[col_latitude] + "~");// latitude
								sb.Append(arrAddress[col_longitude]);		// longitude

								_temparr = new string[] {loc_id.ToString(), usedRadius, curGPS};

								//sr1.WriteLine (sb.ToString());
								htMatched_locs.Add(curGPS, sb.ToString());
								htMatched_address.Add(arrAddress[col_address], _temparr);

								nFoundLocations++;

								sb.Length = 0;

								curLoc_id = loc_id;
								#endregion
							}

							#region append matched NETWORK rows to network.csv file

							foreach (string[] arr in arrResults)
							{
//								if (arrDuplicate.Contains(arr[0])) // mac_id dup check
//									continue;


								net_id++;

								// network_id, location_id, provider, type, ssid, free?, pricing, mac address, [lat, lon, radius]
								sb.Append(net_id + "~"); // network_id
								sb.Append(curLoc_id + "~"); // location_id
								sb.Append(arrAddress[col_provider] + "~"); // provider
								sb.Append("1" + "~");  // type 1=wifi
								sb.Append(arr[1] + "~"); // ssid from wigle
								sb.Append(arrAddress[col_pay] + "~"); // free 0/1
								sb.Append(arrAddress[col_price] + "~"); // pricing $9.99 etc.
								sb.Append(arr[0]/* + "~"*/); // mac address
								//sb.Append(arrAddress[col_latitude] + "~");	// latitude
								//sb.Append(arrAddress[col_longitude] + "~");	// longitude
								//sb.Append(usedRadius);			// radius
					
								//sr2.WriteLine (sb.ToString());

//								if (cbSeperateStates.Checked == true)
//									sr2a.WriteLine(arr);


								if (htMatched_nets.ContainsKey(curLoc_id))
								{
									arrNetwork = (ArrayList) htMatched_nets[curLoc_id];

									foreach (string _itmNet in arrNetwork)
									{	
										string[] _arrNet = _itmNet.Split('~');

										if (_arrNet[7] == arr[0])
										{
											arrNetwork.Remove(_itmNet);
											break;
										}
									}

									arrNetwork.Add(sb.ToString());
//									arrDuplicate.Add(arr[0]);
								}
								else
								{
									arrNetwork = new ArrayList();

									arrNetwork.Add(sb.ToString());
									htMatched_nets.Add(curLoc_id, arrNetwork);
								}

//								sr4.WriteLine(sb.ToString());

								nFoundNetwork++;

								oStatus.Panels[0].Text = sb.ToString();

								sb.Length = 0;
							}
							#endregion
                          
						}
						else
						{
							#region notfound
							if (arrNotFound.Contains(curGPS) == false)
							{
								oStatus.Panels[0].Text = "Skipped: " + arrAddress[col_type] + " " + arrAddress[col_state] + " " + arrAddress[col_city] + " " + arrAddress[col_address];
								//Console.WriteLine(oStatus.Panels[0].Text);
								sr3.WriteLine(row);

								// update listview
								
								lvItem = new ListViewItem(arrAddress);
								listView4.Items.Add(lvItem);

								lvItem.EnsureVisible();

								//listView3.Update();
								//this.Update();

								arrNotFound.Add(curGPS);
								
								nNotFoundLocations++;
							}
							else
								nFoundDuplicate++;
							#endregion
						}

						Application.DoEvents();
					}
					#endregion

					#region closing streams
					trAddress.Close();



					if (cbSeperateStates.Checked == true)
					{
						sr1a.Close();
						sr2a.Close();
					}

					#endregion

					#region append data to files
					foreach (string item in htMatched_locs.Keys)
					{
						row = htMatched_locs[item].ToString();

						sr1.WriteLine(row);

						arrAddress = row.Split('~');

						arrNetwork = (ArrayList) htMatched_nets[Convert.ToInt32(arrAddress[col_type])];

						foreach (string item_net in arrNetwork)
						{
	//						string[] _temp = item_net.Split('~');
	//
	//						if (arrDuplicate.Contains(_temp[7]) == false) // mac_id dup check
	//						{
								sr2.WriteLine(item_net);
	//
	//							arrDuplicate.Add(_temp[7]);
	//						}
						}
					}
					#endregion

				} // end of state loop

				#region finalize
				if (bAbortProcess)
				{
					lblInfo.Text = "Aborted..";

					MessageBox.Show("Matching progress has been cancelled!", "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
				else
				{
					lblInfo.Text = "DONE!..";

					oStatus.Panels[0].Text = curProvider + " has matched.. [" + 
						nProcessedAddresses + " lookup, " + nFoundLocations + " good news, " + 
						nNotFoundLocations + " bad news, " + nFoundDuplicate + " dup skipped, " +
						nFoundNetwork + " mac_id found]";

//					MessageBox.Show(oStatus.Panels[0].Text, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				sr1.Close();
				sr2.Close();
				sr3.Close();
				sr4.Close();
				sr5.Close();
				arrWigle.Clear();

				if (bAbortProcess)
				{
					bAbortProcess = false;
					return;
				}
				#endregion

			} // end of provider loop

		}


	}	
}
