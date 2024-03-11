import React, { useState, useEffect, useContext } from "react";
import { Table, Button, Descriptions, notification, Tag } from "antd";
import { getTaskDetail, getTasks } from "../../../../api/adminTask";
import { UserContext } from "../../../../context/userContext";
import { CKEditor } from "@ckeditor/ckeditor5-react";
import ClassicEditor from "@ckeditor/ckeditor5-build-classic";

const TaskList: React.FC = () => {
  const [tasks, setTasks] = useState<task[]>([]);
  const [showDetail, setShowDetail] = useState<boolean>(false);
  const [selectedTask, setSelectedTask] = useState<detail | null>(null);
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const { token } = useContext(UserContext);
  const userIdFromStorage = localStorage.getItem("userId");
  const userId = userIdFromStorage ? parseInt(userIdFromStorage) : undefined;

  const statusColorMap: { [key: string]: string } = {
    "0": "lightcoral", 
    "1": "blue", 
    "2": "green", 
    "3": "red", 
  };

  const statusStringMap: { [key: string]: string } = {
    "0": "Open", 
    "1": "In Progress", 
    "2": "Result", 
    "3": "Cancel", 
  };

  useEffect(() => {
    fetchTaskList();
  }, [token, userId]);

  const fetchTaskList = async () => {
    try {
      if (token && userId !== undefined) {
        const tasksData = await getTasks(userId, token);
        setTasks(tasksData);
      }
    } catch (error) {
      console.error("Error fetching task list:", error);
      notification.error({
        message: "Error",
        description: "Failed to fetch task list. Please try again.",
      });
    }
  };

  const fetchTaskDetail = async (taskId: number) => {
    try {
      if (token && userId !== undefined) {
        const taskDetailData = await getTaskDetail(userId, taskId, token);

        if (taskDetailData) {
          setSelectedTask(taskDetailData);
          setShowDetail(true);
        }
      }
    } catch (error) {
      console.error("Error fetching task detail:", error);
      notification.error({
        message: "Error",
        description: "Failed to fetch task detail. Please try again.",
      });
    }
  };

  const columns = [
    {
      title: "Created By",
      dataIndex: "accountCreateName",
      key: "accountCreateName",
    },
    {
      title: "Assigned To",
      dataIndex: "accountAssignedName",
      key: "accountAssignedName",
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (status: number) => (
        <Tag color={statusColorMap[String(status)]}>{statusStringMap[String(status)]}</Tag>
      ),
    },
    {
      title: "Date Created",
      dataIndex: "dateCreated",
      key: "dateCreated",
      render: (date: Date) => new Date(date).toLocaleString(),
    },
    {
      title: "Date Updated",
      dataIndex: "dateUpdated",
      key: "dateUpdated",
      render: (date: Date) => new Date(date).toLocaleString(),
    },
    {
      title: "Action",
      key: "action",
      render: (_: any, task: task) => (
        <a onClick={() => fetchTaskDetail(task.taskId)}>View details</a>
      ),
    },
  ];

  const renderBorderedItems = () => {
    const items = [
      {
        key: "1",
        label: "Task",
        children: selectedTask?.taskTitle || "",
        span: 3,
      },
      {
        key: "2",
        label: "Status",
        children: selectedTask ? (
          <Tag color={statusColorMap[selectedTask.status]}>
            {statusStringMap[selectedTask.status]}
          </Tag>
        ) : null,
        span: 3,
      },
      {
        key: "3",
        label: "Assigned To",
        children: selectedTask?.accountAssignedName || "",
        span: 3,
      },
      {
        key: "4",
        label: "Date Created",
        children: selectedTask ? new Date(selectedTask.dateCreated).toLocaleString() : "",
        span: 3,
      },
      {
        key: "5",
        label: "Date Updated",
        children: selectedTask ? new Date(selectedTask.dateUpdated).toLocaleString() : "",
        span: 3,
      },
      {
        key: "6",
        label: "Content",
        children: selectedTask?.taskContent ? (
          <CKEditor
            editor={ClassicEditor}
            data={selectedTask?.taskContent}
            disabled={!isEditing} // Disable CKEditor when not editing
            onChange={(event, editor) => {
              const data = editor.getData();
              setSelectedTask({
                ...selectedTask,
                taskContent: data,
              });
            }}            
          />
        ) : (
          ""
        ),
        span: 3,
      },
      {
        key: "7",
        label: "Notes",
        children: selectedTask?.taskNotes ? (
          <CKEditor
            editor={ClassicEditor}
            data={selectedTask?.taskNotes}
            disabled={!isEditing} // Disable CKEditor when not editing
            onChange={(event, editor) => {
              const data = editor.getData();
              setSelectedTask({
                ...selectedTask,
                taskNotes: data,
              });
            }}            
          />
        ) : (
          ""
        ),
        span: 3,
      },
    ];
  
    return items.map((item) => (
      <Descriptions.Item key={item.key} label={item.label}>
        {item.children}
      </Descriptions.Item>
    ));
  };  

  const handleBackToList = () => {
    setShowDetail(false);
    setSelectedTask(null);
  };

  const handleEditClick = () => {
    setIsEditing(true); // Activate edit mode
  };

  return (
    <>
      {showDetail ? (
        <div>
          <Button onClick={handleBackToList}>Back</Button>
          <Button onClick={handleEditClick}>Edit</Button> {/* Edit button */}
          <br />
          <br />
          <Descriptions bordered title="Task Details">
            {renderBorderedItems()}
          </Descriptions>
        </div>
      ) : (
        <div>
          <Table
            dataSource={tasks}
            columns={columns}
            onRow={(record) => ({
              onClick: () => fetchTaskDetail(record.taskId),
            })}
          />
        </div>
      )}
    </>
  );
};

export default TaskList;
