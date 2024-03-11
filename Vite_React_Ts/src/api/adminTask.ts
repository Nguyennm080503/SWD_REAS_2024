import axios from "axios";
const baseUrl = process.env.REACT_APP_BACK_END_URL;

export const getTasks = async (adminId: number, token: string) => {
  try {
    const paginationParams = {
      
    };

    const taskRequest = {
      AccountCreateName: "", 
      AccountAssignedName: "", 
    };

    const fetchData = await axios.post<task[]>(
      `${baseUrl}/api/admin/${adminId}/tasks`,
      taskRequest,
      {
        params: paginationParams,
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );
    return fetchData.data;
  } catch (error) {
    console.log("Error: " + error);
    throw error;
  }
};

export const getTaskDetail = async (adminId: number, taskId: Number | undefined, token : string) => {
  try {
    const fetchData = await axios.get<detail>(
      `${baseUrl}/api/admin/${adminId}/tasks/${taskId}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );
    return fetchData.data;
  } catch (error) {
    console.log("Error: " + error);
  }
};
