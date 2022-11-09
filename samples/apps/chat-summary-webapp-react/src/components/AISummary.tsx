// Copyright (c) Microsoft. All rights reserved.

import { Body1, Button, Caption1, Spinner, Subtitle1, Subtitle2, Title3 } from '@fluentui/react-components';
import { Card, CardHeader } from '@fluentui/react-components/unstable';
import { FC, useEffect, useState } from 'react';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IKeyConfig } from '../model/KeyConfig';
import { IChatMessage } from './chat/ChatThread';

interface IData {
    uri: string;
    chat: IChatMessage[];
    keyConfig: IKeyConfig;
    onBack: () => void;
}

const AISummary: FC<IData> = ({ uri, chat, keyConfig, onBack }) => {
    const sk = useSemanticKernel(uri);
    const [summary, setSummary] = useState<string>();
    const [actionItems, setActionItems] = useState<string>();
    const [topics, setTopics] = useState<string>();

    const getSummary = async (ask: any) => {
        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'ConversationSummarySkill', 'SummarizeConversation');
            setSummary(result.value);
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };

    const getActionItems = async (ask: any) => {
        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'ConversationSummarySkill', 'GetConversationActionItems');
            setActionItems(result.value);
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };

    const formatActionItems = (actionItems: string): JSX.Element => {
        var actionItemsJson = JSON.parse(
            '[' +
                actionItems
                    .split('}\n\n{')
                    .map((item, index, array) => {
                        if (array.length === 1) {
                            return item;
                        } else if (index === 0) {
                            return item + '}';
                        } else if (index === array.length - 1) {
                            return '{' + item;
                        } else {
                            return '{' + item + '}';
                        }
                    })
                    .join(',') +
                ']',
        );

        var actionItemsList = actionItemsJson.reduce((acc: any, cur: any) => {
            return acc.concat(cur.actionItems);
        }, []);

        var actionItemsFormatted = actionItemsList.map((actionItem: any) => {
            return (
                <Card>
                    <CardHeader header={<Subtitle2>{actionItem.actionItem}</Subtitle2>} />
                    <Body1>
                        <b>Owner:</b> {actionItem.owner}
                    </Body1>
                    <Body1>
                        <b>Due Date:</b> {actionItem.dueDate}
                    </Body1>
                </Card>
            );
        });

        return (
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 20 }}>{actionItemsFormatted}</div>
        );
    };

    const getTopics = async (ask: any) => {
        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'ConversationSummarySkill', 'GetConversationTopics');
            setTopics(result.value);
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };

    const formatTopics = (topics: string): JSX.Element => {
        var topicsJson = JSON.parse(
            '[' +
                topics
                    .split('}\n\n{')
                    .map((item, index, array) => {
                        if (array.length === 1) {
                            return item;
                        } else if (index === 0) {
                            return item + '}';
                        } else if (index === array.length - 1) {
                            return '{' + item;
                        } else {
                            return '{' + item + '}';
                        }
                    })
                    .join(',') +
                ']',
        );

        var topicsList