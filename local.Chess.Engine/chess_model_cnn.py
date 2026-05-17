import torch
import torch.nn as nn


class ChessCNN(nn.Module):
    def __init__(self, in_channels):
        super(ChessCNN, self).__init__()


        self.conv1 = nn.Conv2d(in_channels, out_channels=64, kernel_size=3, padding=1)
        self.bn1 = nn.BatchNorm2d(64)

        self.conv2 = nn.Conv2d(in_channels=64, out_channels=128, kernel_size=3, padding=1)
        self.bn2 = nn.BatchNorm2d(128)

        self.conv3 = nn.Conv2d(in_channels=128, out_channels=128, kernel_size=3, padding=1)
        self.bn3 = nn.BatchNorm2d(128)

        self.reduce = nn.Conv2d(in_channels=128, out_channels=16, kernel_size=1)

        self.fc1 = nn.Linear(16 * 8 * 8, 256)
        self.fc2 = nn.Linear(256, 64)
        self.fc3 = nn.Linear(64, 1)

        self.dropout = nn.Dropout(0.3)

        nn.init.xavier_uniform_(self.fc3.weight, gain=0.01)
        nn.init.zeros_(self.fc3.bias)

    def forward(self, x):
        x = torch.relu(self.bn1(self.conv1(x)))
        x = torch.relu(self.bn2(self.conv2(x)))
        x = torch.relu(self.bn3(self.conv3(x)))

        x = torch.relu(self.reduce(x))

        x = x.view(x.size(0), -1)

        x = torch.relu(self.fc1(x))
        x = self.dropout(x)
        x = torch.relu(self.fc2(x))
        x = self.dropout(x)
        x = torch.tanh(self.fc3(x))
        return x
